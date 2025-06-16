:- dynamic availability/3.%:- retractall(availability(_,_,_)).
:- dynamic availability/3.
:- dynamic timetable/3.
:- dynamic agenda_staff/3.
:- dynamic agenda_staff1/3.
:- dynamic agenda_staff2/3.
:- dynamic agenda_operation_room/3.
:- dynamic assignment_surgery1/2.
:- dynamic agenda_operation_room1/3.
:- dynamic agenda_operation_room2/3.
:- dynamic room_surgeries/2.
:-dynamic generations/1.
:-dynamic population/1.
:-dynamic prob_crossover/1.
:-dynamic prob_mutation/1.
:-dynamic number_passed_elites/1.
:-dynamic tasks/1.
:-dynamic goal_fitness_value/1.
:-dynamic generations_until_stabilization/1.
:-dynamic current_generations_until_stabilization/1.
:-dynamic end_time/1.
%DATA

%EOD
population(2).
number_passed_elites(1).
prob_crossover(30).
prob_mutation(30).
generations(4).
% PopOrd - list with initial generation (tasks, soon to be surgeries)
%
schedule_all_rooms(Day):-
    clear_tables,
    findall(_,(agenda_staff(D,Day,Agenda),assertz(agenda_staff2(D,Day,Agenda))),_),
    findall(_,(agenda_operation_room(D,Day,Agenda),assertz(agenda_operation_room2(D,Day,Agenda))),_),
    get_all_rooms(RCodes),
    assign_surgeries_to_rooms(RCodes),
    schedule_all_rooms(Day, RCodes).
schedule_all_rooms(_,[]).
schedule_all_rooms(Day, [Room| RestRooms]):-
    generate(Room,Day),
    schedule_all_rooms(Day, RestRooms).
clear_genetic_params:-
    retractall(population(_)),
    retractall(number_passed_elites(_)),
    retractall(prob_crossover(_)),
    retractall(prob_mutation(_)),
    retractall(goal_fitness_value(_)),
    retractall(generations_until_stabilization(_)),
    retractall(current_generations_until_stabilization(_)),
    retractall(end_time(_)),
    retractall(generations(_)).
check_room_occupancy(Room):-
    agenda_operation_room2(Room, _, Agenda),
    check_room_ocupancy(Agenda, AgendaTime),
    (room_surgeries(Room, AssignedSurgeries),
     room_surgery_time(AssignedSurgeries, SurgeryTime)
    ; SurgeryTime is 0),
    TotalTime is AgendaTime + SurgeryTime,
    Occupancy is (TotalTime/1440)*100,
    Occupancy < 80.
check_room_ocupancy([], 0).
check_room_ocupancy([(Start,End)|Agenda], TotalOccupancy):-
    Time is End - Start,
    check_room_ocupancy(Agenda, TotalOccupancy1),
    TotalOccupancy is TotalOccupancy1 + Time.
room_surgery_time([],0).
room_surgery_time([Y|X], TotalTime):-
    surgery_id(Y, SurgeryType),
    surgery(SurgeryType, PT, ST, CT),
    SurgeryTime is PT + ST + CT,
    room_surgery_time(X, TotalTime1),
    TotalTime is TotalTime1 + SurgeryTime.
assign_surgeries_to_rooms(RCodes):-
    findall(Surgery, surgery_id(Surgery, _), Surgeries),
    assign_surgeries_to_rooms(RCodes, Surgeries).
assign_surgeries_to_rooms(_, []).
assign_surgeries_to_rooms([Room|RestRooms], [Surgery|RestSurgeries]):-
    check_room_occupancy(Room),!,
    (retract(room_surgeries(Room, AssignedSurgeries)); true),
    append(AssignedSurgeries, [Surgery], UpdatedSurgeries),
    asserta(room_surgeries(Room, UpdatedSurgeries)),
    append(RestRooms, [Room], NewRooms),
    assign_surgeries_to_rooms(NewRooms, RestSurgeries).
assign_surgeries_to_rooms([_|RestRooms], [Surgeries]):-
    assign_surgeries_to_rooms(RestRooms, Surgeries).
get_all_rooms(Rooms) :-
    findall(Room, agenda_operation_room2(Room, _, _), Rooms).
generate(Room, Day):-
    !,
    generate_population(Room,Pop),
    evaluate_population(Room,Day,Pop,PopValue),
    order_population(PopValue,PopOrd),
    generations(NG),
    get_time(Time),
    floor(Time, WholeTime),
    set_random(seed(WholeTime)),
    generate_generation(0,NG,PopOrd,Room,Day).
generate_population(Room,Pop):-
    population(PopSize),
    room_surgeries(Room, SurgeriesList),
    length(SurgeriesList, NumT),
    retractall(tasks(_)),
    asserta(tasks(NumT)),
    generate_population(PopSize,SurgeriesList,NumT,Pop).
generate_population(0,_,_,[]):-!.
generate_population(PopSize,TasksList,NumT,[Ind|Rest]):-
    PopSize1 is PopSize-1,
    generate_population(PopSize1,TasksList,NumT,Rest),
    generate_individual(TasksList,NumT,Ind),
    not(member(Ind,Rest)).
generate_population(PopSize,TasksList,NumT,L):-
    generate_population(PopSize,TasksList,NumT,L).
% Just randomizes the genes of the chromosome..? I guess it's to boost
% randomness which is a "feature" of genetic evolution
generate_individual([G],1,[G]):-!.
generate_individual(TasksList,NumT,[G|Rest]):-
    NumTemp is NumT + 1, % to use with random
    random(1,NumTemp,N),!,
    remove(N,TasksList,G,NewList),
    NumT1 is NumT-1,
    generate_individual(NewList,NumT1,Rest).
remove(1,[G|Rest],G,Rest).
remove(N,[G1|Rest],G,[G1|Rest1]):- N1 is N-1,
            remove(N1,Rest,G,Rest1).
% Traverses the population and evaluates each member, and assigns it
% it's according "fitness" value (priority level, lower better)
evaluate_population(_,_,[],[]).
evaluate_population(Room,Day,[Ind|Rest],[Ind*V|Rest1]):-
    evaluate_room_day(Ind,Room,Day, V),
    evaluate_population(Room, Day, Rest,Rest1).
%Used for when we need to re-evaluate an already evaluated population
re_evaluate_population(_,_,[],[]).
re_evaluate_population(Room,Day,[Ind*_|Rest],[Ind*V|Rest1]):-
    evaluate_room_day(Ind,Room,Day, V),
    re_evaluate_population(Room,Day,Rest,Rest1).
% order_population/2, bsort/2 and bchange/2 are made to order the
% population according to fitness, this is not to be changed
order_population(PopValue,PopValueOrd):-
    bsort(PopValue,PopValueOrd).
bsort([X],[X]):-!.
bsort([X|Xs],Ys):-
    bsort(Xs,Zs),
    bchange([X|Zs],Ys).
bchange([X],[X]):-!.
bchange([X*VX,Y*VY|L1],[Y*VY|L2]):-
    VX>VY,!,
    bchange([X*VX|L1],L2).
bchange([X|L1],[X|L2]):-bchange(L1,L2).
% Evaluates 1 member and assigns it it's fitness value
evaluate(Seq,V):- evaluate(Seq,0,V).
evaluate([ ],_,0).
evaluate([T|Rest],Inst,V):-
    task(T,Dur,Due,Pen),
    FinInst is Inst+Dur,
    evaluate(Rest,FinInst,VRest),
    ((FinInst =< Due,!, VT is 0) ; (VT is (FinInst-Due)*Pen)),
    V is VT+VRest.
% generate_generation/3 does crossover and mutation for the generation,
% then evaluates the population which then uses it to order and generate
% the next generation
generate_generation(G,G,[Ind*_|_],Room,Day):-!,
        schedule_room_day(Ind,Room,Day).
generate_generation(N,G,Pop,Room,Day):-
	!,
        random_permutation(Pop, RandomPop),
	crossover(RandomPop,NPop1),
	mutation(NPop1,NPop),
	evaluate_population(Room,Day,NPop,NPopValue),
        append(NPopValue, Pop, BothPopUnord),
        order_population(BothPopUnord, BothPop),
        population(PS),
        number_passed_elites(NE),
        nFirst(BothPop, NE, Elites),
        remove_list(BothPop, Elites, RestPop),
        randomize_fitness_population(RestPop, RandomizedPop),
        order_population(RandomizedPop, OrderedRandomizedPop),
        PR is PS - NE,
        nFirst(OrderedRandomizedPop, PR, RandomIndividuals),
	append(Elites, RandomIndividuals, NPopRawFit),
	N1 is N+1,
        re_evaluate_population(Room,Day,NPopRawFit, NPopOrdRaw),
        order_population(NPopOrdRaw,[Ind*V|NPopOrd]),!,
        (early_termination(Pop,[Ind*V|NPopOrd]), schedule_room_day(Ind,Room,Day);
	generate_generation(N1,G,[Ind*V|NPopOrd],Room,Day)).
early_termination(OldPop, [Ind*Val|NPopOrdRest]):-
    (goal_fitness_value(GF), GF >= Val);
    (OldPop = [Ind*Val|NPopOrdRest],
     ((retract(current_generations_until_stabilization(SG))),NSG is SG -1,
     asserta(current_generations_until_stabilization(NSG)), NSG =0);
    (OldPop \= [Ind*Val|NPopOrdRest],
     current_generations_until_stabilization(RG), asserta(generations_until_stabilization(RG))),
    end_time(ET), get_time(Time), floor(Time, CurrentTime), ET >= CurrentTime).
randomize_fitness_population([],[]):-!.
randomize_fitness_population([IND*Fit|Rest], [IND*NewFit|Tail]):-
    random(RANDOM),
    NewFitRaw is Fit * RANDOM,
    ceiling(NewFitRaw, NewFit),
    randomize_fitness_population(Rest, Tail).
remove_list([], _, []).
remove_list([X|Tail], L2, Result):- member(X, L2), !, remove_list(Tail, L2, Result).
remove_list([X|Tail], L2, [X|Result]):- remove_list(Tail, L2, Result).
nFirst(List, N, Prefix) :-
    length(List, Len),
    (   Len =< N
    ->  Prefix = List
    ;   length(Prefix, N),
        append(Prefix, _, List)
    ).
% generates the extremities for crossover
generate_crossover_points(P1,P2):- generate_crossover_points1(P1,P2).
generate_crossover_points1(P1,P2):-
	tasks(N),
	NTemp is N+1,
	random(1,NTemp,P11),
	random(1,NTemp,P21),
	P11\==P21,!,
	((P11<P21,!,P1=P11,P2=P21);P1=P21,P2=P11).
generate_crossover_points1(P1,P2):-
	generate_crossover_points1(P1,P2).
% performs crossover between members in the generation pool
crossover([ ],[ ]).
crossover([Ind*_],[Ind]).
crossover([Ind1*_,Ind2*_|Rest],[NInd1,NInd2|Rest1]):-
	generate_crossover_points(P1,P2),
	prob_crossover(Pcruz),random(0.0,1.0,Pc),
	((Pc =< Pcruz,!,
        cross(Ind1,Ind2,P1,P2,NInd1),
	  cross(Ind2,Ind1,P1,P2,NInd2))
	;
	(NInd1=Ind1,NInd2=Ind2)),
	crossover(Rest,Rest1).
fillh([ ],[ ]).
fillh([_|R1],[h|R2]):-
	fillh(R1,R2).
sublist(L1,I1,I2,L):-I1 < I2,!,
    sublist1(L1,I1,I2,L).
sublist(L1,I1,I2,L):-sublist1(L1,I2,I1,L).
sublist1([X|R1],1,1,[X|H]):-!, fillh(R1,H).
sublist1([X|R1],1,N2,[X|R2]):-!,N3 is N2 - 1,
	sublist1(R1,1,N3,R2).
sublist1([_|R1],N1,N2,[h|R2]):-N3 is N1 - 1,
		N4 is N2 - 1,
		sublist1(R1,N3,N4,R2).
rotate_right(L,K,L1):- tasks(N),
	T is N - K,
	rr(T,L,L1).
rr(0,L,L):-!.
rr(N,[X|R],R2):- N1 is N - 1,
	append(R,[X],R1),
	rr(N1,R1,R2).
remove([],_,[]):-!.
remove([X|R1],L,[X|R2]):- not(member(X,L)),!,
        remove(R1,L,R2).
remove([_|R1],L,R2):-
    remove(R1,L,R2).
insert([],L,_,L):-!.
insert([X|R],L,N,L2):-
    tasks(T),
    ((N>T,!,N1 is N mod T);N1 = N),
    insert1(X,N1,L,L1),
    N2 is N + 1,
    insert(R,L1,N2,L2).
insert1(X,1,L,[X|L]):-!.
insert1(X,N,[Y|L],[Y|L1]):-
    N1 is N-1,
    insert1(X,N1,L,L1).
cross(Ind1,Ind2,P1,P2,NInd11):-
    sublist(Ind1,P1,P2,Sub1),
    tasks(NumT),
    R is NumT-P2,
    rotate_right(Ind2,R,Ind21),
    remove(Ind21,Sub1,Sub2),
    P3 is P2 + 1,
    insert(Sub2,Sub1,P3,NInd1),
    removeh(NInd1,NInd11).
removeh([],[]).
removeh([h|R1],R2):-!,
    removeh(R1,R2).
removeh([X|R1],[X|R2]):-
    removeh(R1,R2).
% Performs mutation, mutation only happens if the randomly rolled "Pm"
% value is less than Pmut (mutation chance)
mutation([],[]).
mutation([Ind|Rest],[NInd|Rest1]):-
	prob_mutation(Pmut),
	random(0.0,1.0,Pm),
	((Pm < Pmut,!,mutacao1(Ind,NInd));NInd = Ind),
	mutation(Rest,Rest1).
mutacao1(Ind,NInd):-
	generate_crossover_points(P1,P2),
	mutacao22(Ind,P1,P2,NInd).
mutacao22([G1|Ind],1,P2,[G2|NInd]):-
	!, P21 is P2-1,
	mutacao23(G1,P21,Ind,G2,NInd).
mutacao22([G|Ind],P1,P2,[G|NInd]):-
	P11 is P1-1, P21 is P2-1,
	mutacao22(Ind,P11,P21,NInd).
mutacao23(G1,1,[G2|Ind],G2,[G1|Ind]):-!.
mutacao23(G1,P,[G|Ind],G2,[G|NInd]):-
	P1 is P-1,
	mutacao23(G1,P1,Ind,G2,NInd).
% Operation Scheduling Section
%-GETS THE FREE TIME FROM OCCUPIED TIME INTERVALS--------------------
% free_agenda0(list-occupied-time-intervals,list-free-time-intervals).
free_agenda0([],[(0,1440)]).
free_agenda0([(0,Tfin)|LT],LT1):-!,free_agenda1([(0,Tfin)|LT],LT1).
free_agenda0([(Tin,Tfin)|LT],[(0,T1)|LT1]):- T1 is Tin-1,
    free_agenda1([(Tin,Tfin)|LT],LT1).
free_agenda1([(_,Tfin)],[(T1,1440)]):-Tfin\==1440,!,T1 is Tfin+1.
free_agenda1([(_,_)],[]).
free_agenda1([(_,T),(T1,Tfin2)|LT],LT1):-Tx is T+1,T1==Tx,!,
    free_agenda1([(T1,Tfin2)|LT],LT1).
free_agenda1([(_,Tfin1),(Tin2,Tfin2)|LT],[(T1,T2)|LT1]):-T1 is Tfin1+1,T2 is Tin2-1,
    free_agenda1([(Tin2,Tfin2)|LT],LT1).
% -------------------------------------------------------------------
% ADAPTING A TIMETABLE SO THAT THE FREE TIMES CORRESPOND TO THE TIMES THE PROFESSIONALS WORK FOR.
% adapt_timetable(doctorId, date, list-free-time, list-free-time-during-working-hours).
adapt_timetable(D,Date,LFA,LFA2):-timetable(D,Date,(InTime,FinTime)),treatin(InTime,LFA,LFA1),treatfin(FinTime,LFA1,LFA2).
treatin(InTime,[(In,Fin)|LFA],[(In,Fin)|LFA]):-InTime=<In,!.
treatin(InTime,[(_,Fin)|LFA],LFA1):-InTime>Fin,!,treatin(InTime,LFA,LFA1).
treatin(InTime,[(_,Fin)|LFA],[(InTime,Fin)|LFA]).
treatin(_,[],[]).
treatfin(FinTime,[(In,Fin)|LFA],[(In,Fin)|LFA1]):-FinTime>=Fin,!,treatfin(FinTime,LFA,LFA1).
treatfin(FinTime,[(In,_)|_],[]):-FinTime=<In,!.
treatfin(FinTime,[(In,_)|_],[(In,FinTime)]).
treatfin(_,[],[]).
% -------------------------------------------------------------------
% CHECKS IF THERE ARE ANY FREE AGENDAS FOR ALL STAFF
% find_free_agendas(date).
find_free_agendas(Date):-
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff1(D,Date,L),
    free_agenda0(L,LFA),
    adapt_timetable(D,Date,LFA,LFA2),
    assertz(availability(D,Date,LFA2))),_).
% -------------------------------------------------------------------
% INTERSECTION OF AGENDAS OF FREE TIME INTERVALS OF ALL DOCTORS
% intersect_all_agendas(list-staff-id, date, list-time-intervals-available-for-surgery)
intersect_all_agendas([Name],Date,LA):-!,availability(Name,Date,LA).
intersect_all_agendas([Name|LNames],Date,LI):-
    availability(Name,Date,LA),
    intersect_all_agendas(LNames,Date,LI1),
    intersect_2_agendas(LA,LI1,LI).
intersect_2_agendas([],_,[]).
intersect_2_agendas([D|LD],LA,LIT):-
    intersect_availability(D,LA,LI,LA1),
	intersect_2_agendas(LD,LA1,LID),
	append(LI,LID,LIT).
intersect_availability((_,_),[],[],[]).
intersect_availability((_,Fim),[(Ini1,Fim1)|LD],[],[(Ini1,Fim1)|LD]):-
		Fim<Ini1,!.
intersect_availability((Ini,Fim),[(_,Fim1)|LD],LI,LA):-
		Ini>Fim1,!,
		intersect_availability((Ini,Fim),LD,LI,LA).
intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)],[(Fim,Fim1)|LD]):-
		Fim1>Fim,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_).
intersect_availability((Ini,Fim),[(Ini1,Fim1)|LD],[(Imax,Fmin)|LI],LA):-
		Fim>=Fim1,!,
		min_max(Ini,Ini1,_,Imax),
		min_max(Fim,Fim1,Fmin,_),
		intersect_availability((Fim1,Fim),LD,LI,LA).
min_max(I,I1,I,I1):- I<I1,!.
min_max(I,I1,I1,I).
% -------------------------------------------------------------------
% SCHEDULES ALL SURGERIES IN A SPECIFIC ROOM FOR A PARTICULAR DAY
% schedule_all_surgeries(room-id, date).
% NOTE: For all agenda_staff/3 and agenda_room/3 we create copies of thesefacts to
% be able to handle them (scheduling operations one by one in the agendas) without loosing the initial agendas.
schedule_all_surgeries(Room,Day):-
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    findall(_,(agenda_staff2(D,Day,Agenda),assertz(agenda_staff1(D,Day,Agenda))),_),
    agenda_operation_room2(Or,Date,Agenda),
    assert(agenda_operation_room1(Or,Date,Agenda)),
    findall(_,(agenda_staff1(D,Date,L),
               free_agenda0(L,LFA),
               adapt_timetable(D,Date,LFA,LFA2),
               assertz(availability(D,Date,LFA2))),_),
    findall(OpCode,surgery_id(OpCode,_),LOpCode),
    availability_all_surgeries(LOpCode,Room,Day),!.
evaluate_room_day(LOC,Room,Day, EndTime):-
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    retractall(assignment_surgery1(_,_)),
    asserta(better_sol(Day,Room,_,_,1441)),!,
    findall(_, (agenda_staff2(D,Day,Agenda), assertz(agenda_staff1(D,Day,Agenda))), _),
    findall(_, (assignment_surgery(OpId, StaffId), assertz(assignment_surgery1(OpId, StaffId))),_),
    agenda_operation_room2(Room,Day,Agenda),assert(agenda_operation_room1(Room,Day,Agenda)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOC,Room,Day,EndTime),
    !,
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)).
schedule_room_day(LOC,Room,Day):-
    retractall(agenda_staff1(_,_,_)),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(availability(_,_,_)),
    retractall(assignment_surgery1(_,_)),
    asserta(better_sol(Day,Room,_,_,1441)),!,
    findall(_, (agenda_staff2(D,Day,AgendaS), assertz(agenda_staff1(D,Day,AgendaS))), _),
    findall(_, (assignment_surgery(OpId, StaffId), assertz(assignment_surgery1(OpId, StaffId))),_),
    agenda_operation_room2(Room,Day,AgendaAR),assert(agenda_operation_room1(Room,Day,AgendaAR)),
    findall(_,(agenda_staff1(D,Day,L),free_agenda0(L,LFA),adapt_timetable(D,Day,LFA,LFA2),assertz(availability(D,Day,LFA2))),_),
    availability_all_surgeries(LOC,Room,Day,_),
    !,
    retractall(agenda_staff2(_,_,_)),
    findall(_, (agenda_staff1(D,Day,AgendaS1), assertz(agenda_staff2(D,Day,AgendaS1))), _),
    retractall(agenda_operation_room2(Room,Day,_)),
    findall(_, (agenda_operation_room1(D,Day,AgendaOR1), assertz(agenda_operation_room2(D,Day,AgendaOR1))), _),
    retractall(agenda_operation_room1(_,_,_)),
    retractall(agenda_staff1(_,_,_)),
    retractall(availability(_,_,_)).
% -------------------------------------------------------------------
% AVAILABILITY OF OPERATIONS WILL CREATE A LIST OF POSSIBILITIES IN THE OPERATION ROOM
% availability_all_surgeries(list-operation-id, room-id, date)
availability_all_surgeries([],_,_).
availability_all_surgeries([OpCode],Room,Day, TfinS):-
    find_free_agendas(Day),
    surgery_id(OpCode,OpType),
    surgery(OpType, SurgeryPrepTime, SurgeryProcTime, SurgeryCleanTime),
    TSurgery is SurgeryPrepTime + SurgeryProcTime + SurgeryCleanTime,
    availability_operation(OpCode,Room,Day,LPossibilities),
    schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)),!,
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    findall(Doctor,(assignment_surgery(OpCode,Doctor), staff(Doctor, doctor, _, _)),LDoctors),
    DTinS is TinS + SurgeryPrepTime,
    DTfinS is TfinS - SurgeryCleanTime,
    findall(Cleaner,(assignment_surgery(OpCode,Cleaner), staff(Cleaner, cleaner, _, _)),LCleaners),
    CTinS is TfinS - SurgeryCleanTime,
    CTfinS is TfinS,
    findall(Anaesthesist,(assignment_surgery(OpCode,Anaesthesist), staff(Anaesthesist, anaesthesist, _, _)),LAnaesthesist),
    ATinS is TinS,
    ATfinS is TinS + SurgeryPrepTime + SurgeryProcTime,
    insert_agenda_doctors((DTinS,DTfinS),Day,LDoctors),
    insert_agenda_doctors((CTinS,CTfinS),Day,LCleaners),
    insert_agenda_doctors((ATinS,ATfinS),Day,LAnaesthesist).
availability_all_surgeries([OpCode|LOpCode],Room,Day, EndTime):-
    find_free_agendas(Day),
    surgery_id(OpCode,OpType),
    surgery(OpType, SurgeryPrepTime, SurgeryProcTime, SurgeryCleanTime),
    TSurgery is SurgeryPrepTime + SurgeryProcTime + SurgeryCleanTime,
    availability_operation(OpCode,Room,Day,LPossibilities),
    schedule_first_interval(TSurgery,LPossibilities,(TinS,TfinS)),!,
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS),Agenda,Agenda1),
    assertz(agenda_operation_room1(Room,Day,Agenda1)),
    findall(Doctor,(assignment_surgery(OpCode,Doctor), staff(Doctor, doctor, _, _)),LDoctors),
    DTinS is TinS + SurgeryPrepTime,
    DTfinS is TfinS - SurgeryCleanTime,
    findall(Cleaner,(assignment_surgery(OpCode,Cleaner), staff(Cleaner, cleaner, _, _)),LCleaners),
    CTinS is TfinS - SurgeryCleanTime,
    CTfinS is TfinS,
    findall(Anaesthesist,(assignment_surgery(OpCode,Anaesthesist), staff(Anaesthesist, anaesthesist, _, _)),LAnaesthesist),
    ATinS is TinS,
    ATfinS is TinS + SurgeryPrepTime + SurgeryProcTime,
    insert_agenda_doctors((DTinS,DTfinS),Day,LDoctors),
    insert_agenda_doctors((CTinS,CTfinS),Day,LCleaners),
    insert_agenda_doctors((ATinS,ATfinS),Day,LAnaesthesist),
    availability_all_surgeries(LOpCode,Room,Day,EndTime).
availability_all_surgeries([_|_], _, _, EndTime) :-
    EndTime is 1450,
    true.
% -------------------------------------------------------------------
% GENERATE A LIST OF POSSIBLE TIMES FOR AN OPERATION TO OCCUR IN A SPECIFIED ROOM ON A GIVEN DAY
availability_operation(OpCode,Room,Day,LPossibilities):-
    surgery_id(OpCode,OpType),
    surgery(OpType, PrepTime,SurgeryTime,CleaningTime),
    findall(Doctor,(assignment_surgery(OpCode,Doctor), staff(Doctor, doctor, _, _)),LDoctors),
    findall(Cleaner,(assignment_surgery(OpCode,Cleaner), staff(Cleaner, cleaner, _, _)),LCleaners),
    findall(Anaesthesist,(assignment_surgery(OpCode,Anaesthesist), staff(Anaesthesist, anaesthesist, _, _)),LAnaesthesist),
    intersect_all_agendas(LDoctors,Day,DoctorGroupAgenda),
    remove_unf_intervals(SurgeryTime,DoctorGroupAgenda, ValidSurgeryTimes),
    section_start_times_for_phase(SurgeryTime, ValidSurgeryTimes, ValidSurgeryStartTimes),
    SurgeryShift is PrepTime,
    shift_agenda_left(ValidSurgeryStartTimes, SurgeryShift, DoctorAgenda),
    intersect_all_agendas(LCleaners,Day,LC),
    remove_unf_intervals(CleaningTime, LC, ValidCleaningTimes),
    section_start_times_for_phase(CleaningTime, ValidCleaningTimes, ValidCleaningStartTimes),
    CleaningShift is SurgeryTime + PrepTime,
    shift_agenda_left(ValidCleaningStartTimes, CleaningShift, CleanerAgenda),
    intersect_all_agendas(LAnaesthesist,Day,LA),
    AnaesthesiaTime is PrepTime + SurgeryTime,
    remove_unf_intervals(AnaesthesiaTime , LA, ValidAenesthesiaTimes),
    section_start_times_for_phase(AnaesthesiaTime, ValidAenesthesiaTimes, AnaesthesistAgenda),
    agenda_operation_room1(Room,Day,LAgenda),
    free_agenda0(LAgenda,LOp),
    TotalTime is PrepTime + SurgeryTime + CleaningTime,
    remove_unf_intervals(TotalTime, LOp, LFAgRoom),
    section_start_times_for_phase(TotalTime, LFAgRoom, RoomAgenda),
    intersect_2_agendas(DoctorAgenda,RoomAgenda,LIntAgDoctorsRoom),
    intersect_2_agendas(CleanerAgenda,LIntAgDoctorsRoom,LIntAgCleanersRoom),
    intersect_2_agendas(AnaesthesistAgenda,LIntAgCleanersRoom,LIntFinalRoom),
    actualize_surgery_time(TotalTime, LIntFinalRoom,LPossibilities).
section_start_times_for_phase(_, [], []):-!.
section_start_times_for_phase(Duration, [(Start, R)|Slots], [(Start, End)|StartSlots]):-
    End is R - Duration,
    section_start_times_for_phase(Duration, Slots, StartSlots).
actualize_surgery_time(_, [], []):-!.
actualize_surgery_time(Duration, [(Start, _)|Slots], [(Start, End)|SurgerySlots]):-
    End is Start + Duration,
    actualize_surgery_time(Duration, Slots, SurgerySlots).
valid_time(Prep, Surgery, Clean, Time):-
    (Prep=<Surgery, Prep=<Clean, Time is Prep, !);
    (Clean=<Surgery, Clean=<Prep, Time is Clean, !);
    Time is Surgery.
% -------------------------------------------------------------------
% REMOVES UNFEASIBLE TIME INTERVALS FROM THE RESULTING AGENDA
% remove_unf_intervals(surgery-time, list-intersected-agendas-doctor, list-availability).
remove_unf_intervals(_,[],[]).
remove_unf_intervals(TSurgery,[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-DT is Tfin-Tin,TSurgery=<DT,!,
    remove_unf_intervals(TSurgery,LA,LA1).
remove_unf_intervals(TSurgery,[_|LA],LA1):- remove_unf_intervals(TSurgery,LA,LA1).
% -------------------------------------------------------------------
% SCHEDULES THE FIRST AVAILABLE TIME INTERVAL
schedule_first_interval(TSurgery,[(Tin,_)|_],(Tin,TfinS)):-
    TfinS is Tin + TSurgery - 1.
% -------------------------------------------------------------------
% INSERTS OPERATION INTO THE DOCTORS AND ROOM AGENDA
insert_agenda((TinS,TfinS),[],[(TinS,TfinS)]).
insert_agenda((TinS,TfinS),[(Tin,Tfin)|LA],[(TinS,TfinS),(Tin,Tfin)|LA]):-TfinS<Tin,!.
insert_agenda((TinS,TfinS),[(Tin,Tfin)|LA],[(Tin,Tfin)|LA1]):-insert_agenda((TinS,TfinS),LA,LA1).
insert_agenda_doctors(_,_,[]).
insert_agenda_doctors((TinS,TfinS),Day,[Doctor|LDoctors]):-
    retract(agenda_staff1(Doctor,Day,Agenda)),
    insert_agenda((TinS,TfinS),Agenda,Agenda1),
    assert(agenda_staff1(Doctor,Day,Agenda1)),
    insert_agenda_doctors((TinS,TfinS),Day,LDoctors).
insert_agenda_room(_,_,[]).
insert_agenda_room((TinS,TfinS,OpCode),Day,Room):-
    retract(agenda_operation_room1(Room,Day,Agenda)),
    insert_agenda((TinS,TfinS,OpCode),Agenda,Agenda1),
    assert(agenda_operation_room1(Room,Day,Agenda1)).
% -------------------------------------------------------------------
evaluate_final_time([],_,1441).
evaluate_final_time([(_,Tfin,OpCode)|_],LOpCode,Tfin):-member(OpCode,LOpCode),!.
evaluate_final_time([_|AgR],LOpCode,Tfin):-evaluate_final_time(AgR,LOpCode,Tfin).
list_doctors_agenda(_,[],[]).
list_doctors_agenda(Day,[D|LD],[(D,AgD)|LAgD]):-
agenda_staff1(D,Day,AgD),list_doctors_agenda(Day,LD,LAgD).
remove_equals([],[]).
remove_equals([X|L],L1):-member(X,L),!,remove_equals(L,L1).
remove_equals([X|L],[X|L1]):-remove_equals(L,L1).
% Utilities
shift_agenda_left([], _, []):-!.
shift_agenda_left([(Start, End)| Rest], Shift,[(NewStart, NewEnd)| NewRest] ):-
    NewStart is Start - Shift,
    NewEnd is End - Shift,
    shift_agenda_left(Rest, Shift, NewRest).
    % Predicate to clear all entries from agenda_staff1 and agenda_operation_room1
clear_tables:-
    retractall(room_surgeries(_,_)),
    retractall(agenda_staff1(_, _, _)),
    retractall(agenda_staff2(_,_,_)),
    retractall(agenda_operation_room2(_, _, _)),
    retractall(assignment_surgery1(_,_)).
% Predicate to print all agenda_staff2 facts
print_all_agenda_staff :-
    findall((Staff, Day, Agenda), agenda_staff2(Staff, Day, Agenda), Results),
    writeln('Agenda Staff Information:'),
    print_list(Results).
% Predicate to print all agenda_operation_room facts
print_all_agenda_operation_room :-
    findall((Room, Day, Agenda), agenda_operation_room2(Room, Day, Agenda), Results),
    writeln('Agenda Operation Room Information:'),
    print_list(Results).
% Helper predicate to print a list of results
print_list([]).
print_list([Head|Tail]) :-
    writeln(Head),
    print_list(Tail).
% Wrapper predicate to print both sets of information
print_all_agendas :-
    print_all_agenda_staff,
    writeln(''), % Print a blank line for readability
    print_all_agenda_operation_room.
