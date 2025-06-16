/* import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationTypeCreationComponent } from './admin-operation-type-creation.component';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BehaviorSubject } from 'rxjs';
import { of } from 'rxjs';
import { LoginResponse } from '../../login-result';
import { OperationPhase, RequiredSpecialist } from '../interfaces/operation-type-data';

describe('OperationTypeCreationComponent', () => {
  let component: OperationTypeCreationComponent;
  let fixture: ComponentFixture<OperationTypeCreationComponent>;
  let adminService: AdminService;
  let router: Router;
  let httpMock: HttpTestingController;

  const mockToken = 'mock-token';
  const mockSpecializations = ['Cardiology', 'Neurology', 'Orthopedics'];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, OperationTypeCreationComponent],
      declarations: [],
      providers: [
        AdminService,
        { provide: Router, useValue: { navigate: jasmine.createSpy() } },
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationTypeCreationComponent);
    component = fixture.componentInstance;
    adminService = TestBed.inject(AdminService);
    router = TestBed.inject(Router);
    httpMock = TestBed.inject(HttpTestingController);

    component.response = { Result: "AUTH", Token: "fjkdsnndsfajk", Type: '  '};
    localStorage.setItem('authToken', 'mock-token');
    spyOn(adminService, 'getAllSpecializations').and.callFake(() => {
      return of(mockSpecializations);
    });
    spyOn(adminService, 'validate').and.callFake(() => {
      return of({ role: 'ADMIN' });
    });
    
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });


  it('should add required specialist to the list', () => {
    component.requiredSpecialistHelper[0] = { specialization: 'Cardiology', count: 2, phase: 'Preparation' };
    component.addRequiredSpecialist(0);

    expect(component.requiredSpecialists.length).toBe(1);
    expect(component.requiredSpecialists[0]).toEqual({ specialization: 'Cardiology', count: 2, phase: 'Preparation' });
  });

  it('should not add required specialist if input is invalid', () => {
    component.requiredSpecialistHelper[0] = { specialization: '', count: 0, phase: 'Preparation' };
    component.addRequiredSpecialist(0);

    expect(component.requiredSpecialists.length).toBe(0);
  });

  it('should create an operation type', () => {
    const operationTypeData = {
      name: 'Heart Surgery',
      estimatedDuration: 120,
      phases: [{ phaseName: 'Preparation', duration: 30 }],
      specialists: [{ specialization: 'Cardiology', count: 2, phase: 'Preparation' }],
    };
  
    component.operationType = operationTypeData as any;
  
    spyOn(adminService, 'createOperationType').and.callThrough();
  
    component.createOperationType();
  
    expect(adminService.createOperationType).toHaveBeenCalledWith(
      'mock-token',
      'Heart Surgery',
      120,
      ['Preparation', 'Surgery', 'Cleaning'],
      ['0', '0', '0'],
      [],
      [],
      []
    );
  
    const req = httpMock.expectOne('https://localhost:5001/api/OperationType/createOperation');
    expect(req.request.method).toBe('POST');
  
    req.flush({ message: 'Operation type created successfully' });
    httpMock.verify();
  });
  

  it('should reset the form after creating an operation type', () => {
    const operationTypeData = {
      name: 'Heart Surgery',
      estimatedDuration: 120,
      phases: [{ phaseName: 'Preparation', duration: 30 }],
      specialists: [{ specialization: 'Cardiology', count: 2, phase: 'Preparation' }],
    };
  
    component.operationType = operationTypeData as any;
  
    component.createOperationType();
  
    expect(component.operationPhases).toEqual([
      { phaseName: 'Preparation', duration: 0 },
      { phaseName: 'Surgery', duration: 0 },
      { phaseName: 'Cleaning', duration: 0 },
    ]);
    expect(component.requiredSpecialists).toEqual([]);
    expect(component.operationType).toEqual({ name: '', phases: [], specialists: [], estimatedDuration: 0 });
  
    const req = httpMock.expectOne('https://localhost:5001/api/OperationType/createOperation');
    expect(req.request.method).toBe('POST');
  
    req.flush({ message: 'Operation type created successfully' });
  
    httpMock.verify();
  });
  

  it('should call validate and check admin role', () => {
    component.ngOnInit();

    expect(adminService.validate).toHaveBeenCalledWith(mockToken);
    expect(router.navigate).not.toHaveBeenCalled();
  });

});
 */