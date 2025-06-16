import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AvailabilitySlots } from './hospital-data';
import { environment } from '../../environments/local.environments.prod';
import { AppointmentDto } from '../Admin/interfaces/operation-type-data';

@Injectable({
  providedIn: 'root'
})
export class HospitalService {

  private apiUrl = environment.apiUrl + 'OperationRequest/';
  private appointmentURL = environment.apiUrl + 'Appointment/';

  constructor(private http: HttpClient) {}

  getAllRoomAvailabilitySlots(): Observable<AvailabilitySlots[]> {
    return this.http.get<AvailabilitySlots[]>(`${this.apiUrl}AllRoomAvailabilitySlots`);
  }

  getAppointmentByRoom(roomID: string): Observable<AppointmentDto> {
    let token = localStorage.getItem('authToken');
    if(token){

      const headers = new HttpHeaders({
        'auth': token,
      });
      
      return this.http.get<AppointmentDto>(`${this.appointmentURL}AppointmentByRoom?operationRoom=${roomID}`, {headers: headers})
    }else{
      throw "what";
    }
  }
}
