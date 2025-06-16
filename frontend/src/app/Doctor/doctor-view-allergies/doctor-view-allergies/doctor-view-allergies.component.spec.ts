import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DoctorViewAllergiesComponent } from './doctor-view-allergies.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DoctorService } from '../../../Doctor/doctor.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { AllergyData } from '../../../Admin/interfaces/allergy-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

describe('DoctorViewAllergiesComponent', () => {
  let component: DoctorViewAllergiesComponent;
  let fixture: ComponentFixture<DoctorViewAllergiesComponent>;
  let doctorService: jasmine.SpyObj<DoctorService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('DoctorService', ['getAllergies']);

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, HttpClientTestingModule, CommonModule, FormsModule],
      providers: [
        FormBuilder,
        { provide: DoctorService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DoctorViewAllergiesComponent);
    component = fixture.componentInstance;
    doctorService = TestBed.inject(DoctorService) as jasmine.SpyObj<DoctorService>;

    // Mock the method to return observables
    doctorService.getAllergies.and.returnValue(of([])); // Mocking getAllergies with an empty array
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize token from localStorage on ngOnInit', () => {
    spyOn(localStorage, 'getItem').and.returnValue('mock-token');
    component.ngOnInit();
    expect(component.token).toBe('mock-token');
  });

  it('should call getAllergies and set allergies correctly', () => {
    const allergiesMock: AllergyData[] = [{ id: '1', name: 'Peanuts', description: 'Peanuts allergy' }];
    doctorService.getAllergies.and.returnValue(of(allergiesMock)); // Mocking the response

    component.getAllergies();
    expect(component.allergies).toEqual(allergiesMock);
    expect(component.statusMessage).toBe('Allergies fetched successfully');
  });

  it('should set status message when getAllergies fails', () => {
    const errorResponse = { message: 'Error fetching allergies' };
    doctorService.getAllergies.and.returnValue(throwError(errorResponse)); // Mocking error

    component.getAllergies();
    expect(component.statusMessage).toBe(`Error fetching allergies: ${errorResponse.message}`);
  });

  it('should reset queryData after fetching allergies', () => {
    const allergiesMock: AllergyData[] = [{ id: '1', name: 'Peanuts', description: 'Peanuts allergy' }];
    doctorService.getAllergies.and.returnValue(of(allergiesMock)); // Mocking the response

    component.getAllergies();
    expect(component.queryData).toEqual({ id: '', name: '', description: '' });
  });

  it('should set selectedAllergy when fetchAllergyById is called', () => {
    const allergyMock: AllergyData = { id: '1', name: 'Peanuts', description: 'Peanuts allergy' };
    component.allergies = [allergyMock];

    component.fetchAllergyById('1');
    expect(component.selectedAllergy).toEqual(allergyMock);
  });

  it('should set selectedAllergy to null when fetchAllergyById does not find the allergy', () => {
    component.fetchAllergyById('non-existent-id');
    expect(component.selectedAllergy).toBeNull();
  });
});
