import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { MedicalConditionControllerComponent } from './medical-condition-controller.component';
import { AdminService } from '../admin.service';
import { MedicalConditionData } from '../interfaces/medical-condition-data';

describe('MedicalConditionControllerComponent', () => {
  let component: MedicalConditionControllerComponent;
  let fixture: ComponentFixture<MedicalConditionControllerComponent>;
  let adminServiceMock: jasmine.SpyObj<AdminService>;

  beforeEach(async () => {
    adminServiceMock = jasmine.createSpyObj('AdminService', ['getMedicalConditions', 'createMedicalCondition']);

    await TestBed.configureTestingModule({
      imports: [MedicalConditionControllerComponent],
      providers: [
        FormBuilder,
        { provide: AdminService, useValue: adminServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalConditionControllerComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should set token from localStorage and refresh conditions', () => {
      spyOn(localStorage, 'getItem').and.returnValue('testToken');
      const refreshSpy = spyOn(component, 'refreshConditions');

      component.ngOnInit();

      expect(component.token).toBe('testToken');
      expect(refreshSpy).toHaveBeenCalled();
    });
  });

  describe('getMedicalConditions', () => {
    it('should fetch medical conditions and set them on success', () => {
      const mockConditions: MedicalConditionData[] = [
        { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' }
      ];

      adminServiceMock.getMedicalConditions.and.returnValue(of(mockConditions));
      component.queryData = { code: '', designation: '', description: '', symptoms: '' };

      component.getMedicalConditions();

      expect(adminServiceMock.getMedicalConditions).toHaveBeenCalledWith(component.token, component.queryData);
      expect(component.conditions).toEqual(mockConditions);
      expect(component.statusMessage).toBe('');
    });

  });

  describe('createMedicalCondition', () => {
    it('should create a medical condition and refresh the list on success', () => {
      const mockCondition: MedicalConditionData = {
        code: '1',
        designation: 'Asthma',
        description: 'Chronic condition',
        symptoms: 'Coughing'
      };

      adminServiceMock.createMedicalCondition.and.returnValue(of(mockCondition));
      const resetSpy = spyOn(component, 'resetData');
      const refreshSpy = spyOn(component, 'refreshConditions');

      component.createData = mockCondition;
      component.createMedicalCondition();

      expect(adminServiceMock.createMedicalCondition).toHaveBeenCalledWith(component.token, mockCondition);
      expect(component.sucessMessage).toBe('Medical Condition created successfully');
      expect(component.errorMessage).toBe('');
      expect(resetSpy).toHaveBeenCalled();
      expect(refreshSpy).toHaveBeenCalled();
    });

    it('should handle validation errors and set errorMessage', () => {
      const errorResponse = new HttpErrorResponse({
        status: 500,
        statusText: 'Internal Server Error'
      });

      adminServiceMock.createMedicalCondition.and.returnValue(throwError(() => errorResponse));
      component.createData = { code: '', designation: '', description: '', symptoms: '' };

      component.createMedicalCondition();

      expect(component.errorMessage).toBe('Error creating Medical Condition: All the spaces which aren\'t optional need to be filled.');
      expect(component.sucessMessage).toBe('');
    });
  });

  describe('resetData', () => {
    it('should reset queryData, createData, and selectedCondition', () => {
      component.queryData = { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' };
      component.createData = { code: '2', designation: 'Flu', description: 'Viral infection', symptoms: 'Fever' };
      component.selectedCondition = { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' };

      component.resetData();

      expect(component.queryData).toEqual({ code: '', designation: '', description: '', symptoms: '' });
      expect(component.createData).toEqual({ code: '', designation: '', description: '', symptoms: '' });
      expect(component.selectedCondition).toBeNull();
    });
  });

  describe('refreshConditions', () => {
    it('should call getMedicalConditions', () => {
      const getMedicalConditionsSpy = spyOn(component, 'getMedicalConditions');

      component.refreshConditions();

      expect(getMedicalConditionsSpy).toHaveBeenCalled();
    });
  });

  describe('fetchConditionById', () => {
    it('should set selectedCondition based on the given code', () => {
      component.conditions = [
        { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' },
        { code: '2', designation: 'Flu', description: 'Viral infection', symptoms: 'Fever' }
      ];

      component.fetchConditionById('1');

      expect(component.selectedCondition).toEqual({ code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' });
    });

    it('should set selectedCondition to null if no match is found', () => {
      component.conditions = [
        { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' }
      ];

      component.fetchConditionById('2');

      expect(component.selectedCondition).toBeNull();
    });
  });
});
