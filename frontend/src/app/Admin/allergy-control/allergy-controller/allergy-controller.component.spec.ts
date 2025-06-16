import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AllergyControllerComponent } from './allergy-controller.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AdminService } from '../../admin.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { AllergyData } from '../../interfaces/allergy-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

describe('AllergyControllerComponent', () => {
  let component: AllergyControllerComponent;
  let fixture: ComponentFixture<AllergyControllerComponent>;
  let adminService: jasmine.SpyObj<AdminService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('AdminService', ['getAllergies', 'patchAllergies', 'createAllergy']);

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, HttpClientTestingModule, CommonModule, FormsModule],
      providers: [
        FormBuilder,
        { provide: AdminService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AllergyControllerComponent);
    component = fixture.componentInstance;
    adminService = TestBed.inject(AdminService) as jasmine.SpyObj<AdminService>;

    // Mock the methods to return observables
    adminService.getAllergies.and.returnValue(of([])); // Mocking getAllergies with an empty array
    adminService.patchAllergies.and.returnValue(of({id: '', name: '', description: ''})); // Mocking patchAllergies with an empty object
    adminService.createAllergy.and.returnValue(of({id: '', name: '', description: ''})); // Mocking createAllergy with an empty object
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
    adminService.getAllergies.and.returnValue(of(allergiesMock)); // Mocking the response

    component.getAllergies();
    expect(component.allergies).toEqual(allergiesMock);
  });

  it('should set status message when getAllergies fails', () => {
    const errorResponse = { message: 'Error fetching allergies' };
    adminService.getAllergies.and.returnValue(throwError(errorResponse)); // Mocking error

    component.getAllergies();
    expect(component.statusMessage).toBe(`Error fetching allergies: ${errorResponse.message}`);
  });

  it('should call patchAllergies and update the status message', () => {
    const allergyMock: AllergyData = { id: '1', name: 'Peanuts', description: 'Peanuts allergy' };
    component.selectedAllergy = allergyMock;
    adminService.patchAllergies.and.returnValue(of(allergyMock)); // Mocking the response

    component.patchAllergies();
    expect(component.statusMessage).toBe('Allergy updated successfully');
  });

  it('should set status message when patchAllergies fails', () => {
    const errorResponse = { message: 'Error updating allergy' };
    adminService.patchAllergies.and.returnValue(throwError(errorResponse)); // Mocking error

    component.patchAllergies();
    expect(component.statusMessage).toBe(null);
  });

  it('should call createAllergy and set success status message', () => {
    const allergyMock: AllergyData = { id: '1234.A', name: 'Peanuts', description: 'Peanuts allergy' };
    component.createData = allergyMock;
    adminService.createAllergy.and.returnValue(of(allergyMock)); // Mocking the response

    component.createAllergy();
    expect(component.statusMessage).toBe('Allergy created successfully.');
  });

  it('should set status message when createAllergy fails', () => {
    const errorResponse = { message: 'Error creating allergy' };
    adminService.createAllergy.and.returnValue(throwError(errorResponse)); // Mocking error

    component.createAllergy();
    expect(component.statusMessage).toBe(`Error creating allergy: ${errorResponse.message}`);
  });

  it('should reset queryData after creating an allergy', () => {
    const allergyMock: AllergyData = { id: '1234.A', name: 'Peanuts', description: 'Peanuts allergy' };
    component.createData = allergyMock;
    adminService.createAllergy.and.returnValue(of(allergyMock)); // Mocking the response

    component.createAllergy();
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
