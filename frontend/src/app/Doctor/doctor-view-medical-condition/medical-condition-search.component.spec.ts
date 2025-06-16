import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { MedicalConditionSearchComponent } from './medical-condition-search.component';
import { DoctorService } from '../doctor.service';
import { MedicalConditionData } from '../interfaces/medical-condition-data';

describe('MedicalConditionSearchComponent', () => {
  let component: MedicalConditionSearchComponent;
  let fixture: ComponentFixture<MedicalConditionSearchComponent>;
  let doctorServiceMock: jasmine.SpyObj<DoctorService>;

  beforeEach(async () => {
    doctorServiceMock = jasmine.createSpyObj('DoctorService', ['getMedicalConditions']);

    await TestBed.configureTestingModule({
      imports: [MedicalConditionSearchComponent],
      providers: [
        FormBuilder,
        { provide: DoctorService, useValue: doctorServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalConditionSearchComponent);
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

      doctorServiceMock.getMedicalConditions.and.returnValue(of(mockConditions));
      component.queryData = { code: '1', designation: '', description: '', symptoms: '' };

      component.getMedicalConditions();

      expect(doctorServiceMock.getMedicalConditions).toHaveBeenCalledWith(component.token, component.queryData);
      expect(component.conditions).toEqual(mockConditions);
      expect(component.errorMessage).toBe('');
    });

    it('should set errorMessage if the code is non-numeric', () => {
      component.queryData.code = 'abc';

      component.getMedicalConditions();

      expect(component.errorMessage).toBe('Error fetching medical conditions: Code can only be numeric.');
      expect(doctorServiceMock.getMedicalConditions).not.toHaveBeenCalled();
    });

    it('should handle errors from the service', () => {
      const errorResponse = new HttpErrorResponse({
        error: { message: 'Unauthorized' },
        status: 401
      });

      doctorServiceMock.getMedicalConditions.and.returnValue(throwError(() => errorResponse));
      component.queryData = { code: '1', designation: '', description: '', symptoms: '' };

      component.getMedicalConditions();

      expect(component.errorMessage).toBe('Error fetching medical conditions: Unauthorized');
    });
  });

  describe('resetData', () => {
    it('should reset queryData and selectedCondition', () => {
      component.queryData = { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' };
      component.selectedCondition = { code: '1', designation: 'Asthma', description: 'Chronic condition', symptoms: 'Coughing' };

      component.resetData();

      expect(component.queryData).toEqual({ code: '', designation: '', description: '', symptoms: '' });
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
