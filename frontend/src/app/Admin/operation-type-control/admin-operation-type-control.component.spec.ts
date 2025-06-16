/* import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationTypeControlComponent } from './admin-operation-type-control.component';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('OperationTypeControlComponent', () => {
  let component: OperationTypeControlComponent;
  let fixture: ComponentFixture<OperationTypeControlComponent>;
  let mockAdminService: jasmine.SpyObj<AdminService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockAdminService = jasmine.createSpyObj('AdminService', [
      'validate',
      'getAllSpecializations',
      'getOperationTypeFiltered',
      'deleteOperationType',
      'patchOperationType',
    ]);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [OperationTypeControlComponent, FormsModule, CommonModule],
      providers: [
        { provide: AdminService, useValue: mockAdminService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationTypeControlComponent);
    component = fixture.componentInstance;

    // Mock localStorage
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'authToken') return 'test-token';
      return null;
    });

    // Default mocks
    mockAdminService.validate.and.returnValue(of({ role: 'ADMIN' }));
    mockAdminService.getAllSpecializations.and.callFake(() => {
      mockAdminService.specialization$ = of(['Specialist1', 'Specialist2']);
    });
    mockAdminService.getOperationTypeFiltered.and.callFake(() => {
      mockAdminService.operationTypeResults$ = of([
        {
          ID: '1',
          OperationName: 'Test Operation',
          RequiredSpecialists: ['Spec1:2,Preparation', 'Spec2:1,Surgery'],
          EstimatedDuration: "120",
          PhaseNames: ['Preparation', 'Surgery'],
          PhasesDuration: ["30", "90"],
          OperationStartDate: '',
          OperationEndDate: '',
          VersionNumber: '',
          ActivationStatus: '',
          OperationPhases: [],
          SpecialistNames: [],
          SpecialistsCount: [],
          SpecialistsPhases: [],
        },
      ]);
    });

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize data on ngOnInit', () => {
    const testToken = 'test-token';
    component.storedToken = testToken;
  
    // Mock validate to return a valid admin response
    mockAdminService.validate.and.returnValue(
      of({ role: 'ADMIN', token: testToken })
    );
  
    // Act
    component.ngOnInit();
  
    // Assert
    expect(mockAdminService.validate).toHaveBeenCalledWith(testToken);
    expect(mockAdminService.getAllSpecializations).toHaveBeenCalledWith(testToken);
    expect(mockAdminService.getOperationTypeFiltered).toHaveBeenCalledWith(
      testToken,
      '',
      '',
      ''
    );
  
    expect(component.specializations).toEqual(['Specialist1', 'Specialist2']);
    expect(component.operationTypeResults.length).toBe(1);
  });

  it('should navigate to login if no token is present', () => {
    // Clear the token to simulate the no-token scenario
    localStorage.clear();
    component.storedToken = null; // Ensure storedToken is null
  
    // Act
    component.ngOnInit();
  
    // Assert
    expect(mockRouter.navigate).toHaveBeenCalledWith(['']);
  });

  it('should get operation details when gotOperation is called', () => {
    component.operationTypeResults = [
      {
        ID: '1',
        OperationName: 'Test Operation',
        RequiredSpecialists: ['Spec1:2,Preparation', 'Spec2:1,Surgery'],
        EstimatedDuration: "120",
        PhaseNames: ['Preparation', 'Surgery'],
        PhasesDuration: ["30", "90"],
        OperationStartDate: '',
        OperationEndDate: '',
        VersionNumber: '',
        ActivationStatus: '',
        OperationPhases: [],
        SpecialistNames: [],
        SpecialistsCount: [],
        SpecialistsPhases: []
      },
    ];

    component.gotOperation('1');
    expect(component.operationTypeResult).toEqual(component.operationTypeResults[0]);
    expect(component.requireSpecialistChangeHelper).toContain('Spec1:2,Preparation');
  });

  it('should filter operation types', () => {
    // Arrange
    const mockToken = 'test-token';
    component.storedToken = mockToken; // Simulate token storage
    component.operationTypeQuery = {
      name: 'Test',
      specialization: 'Specialist1',
      status: 'Active',
    };
  
    // Mock the service response
    mockAdminService.getOperationTypeFiltered.and.callFake(() => {
      return;
    });
    // Act
    component.findOperationType();
  
    // Assert
    expect(mockAdminService.getOperationTypeFiltered).toHaveBeenCalledWith(
      mockToken,
      'Test',
      'Specialist1',
      'Active'
    );
  });

  it('should deactivate an operation', () => {
    // Arrange
    component.storedToken = 'test-token'; // Ensure storedToken is correctly set
  
    // Set operationTypeResult to simulate a selected operation
    component.operationTypeResult = {
      ID: '1',
      OperationName: 'Test Operation',
      RequiredSpecialists: [],
      EstimatedDuration: "120",
      PhaseNames: [],
      PhasesDuration: [],
      OperationStartDate: '',
      OperationEndDate: '',
      VersionNumber: '',
      ActivationStatus: '',
      OperationPhases: [],
      SpecialistNames: [],
      SpecialistsCount: [],
      SpecialistsPhases: []
    };
  
    mockAdminService.deleteOperationType.and.returnValue(); 
    
    // Act
    component.deactivateOperation();
  
    expect(mockAdminService.deleteOperationType).toHaveBeenCalledWith(
      'test-token', // This should match the token set in the component
      'Test Operation' // The operation name should also match
    );
    expect(component.operationTypeResult).toBeNull(); 
  });

  /*it('should call patchOperationType with the correct arguments when patchOperation is called', () => {
    // Arrange
    const mockToken = 'test-token'; // Set the mock token
    component.storedToken = mockToken;
  
    // Set up the operationTypeResult with necessary values
    component.operationTypeResult = {
      ID: '1',
      OperationName: 'Test Operation',
      RequiredSpecialists: [],
      EstimatedDuration: '120',
      PhaseNames: ['Preparation', 'Surgery'],
      PhasesDuration: ['30', '90'],
      OperationStartDate: '',
      OperationEndDate: '',
      VersionNumber: '',
      ActivationStatus: '',
      OperationPhases: [], // Simulate the OperationPhases
      SpecialistNames: [],
      SpecialistsCount: [],
      SpecialistsPhases: []
    };
  
    // Set the requireSpecialistChangeHelper to simulate specialist details
    component.requireSpecialistChangeHelper = 'Spec1: 2, Preparation\nSpec2: 1, Surgery\nSpec3: 3, Cleaning'; // This line has malformed input for "Spec3"
  
    // Set up the operationPhases to simulate phases and their durations
    component.operationPhases = [
      { phaseName: 'Preparation', duration: 30 },
      { phaseName: 'Surgery', duration: 90 },
      { phaseName: 'Cleaning', duration: 60 } // Add Cleaning phase to match the helper
    ];
  
    // Mock the patchOperationType method to return an observable (you can simulate success/failure here)
    mockAdminService.patchOperationType.and.returnValue(); // Mocking the return value as success
  
    // Spy on console.log to check for any console logs (optional)
    spyOn(console, 'log');
  
    // Act
    component.patchOperation();
  
    // Assert
    expect(mockAdminService.patchOperationType).toHaveBeenCalledWith(
      'test-token', // token passed in patchOperationType call
      'Test Operation', // operation name
      '120', // estimated duration
      ['Preparation', 'Surgery', 'Cleaning'], // phase names (includes 'Cleaning')
      ['30', '90', '60'], // phase durations (includes duration for 'Cleaning')
      ['Spec1', 'Spec2'], // specialist names (without 'Spec3' due to malformed input)
      ['2', '1'], // specialist counts
      ['Preparation', 'Surgery'] // specialist phases (excludes 'Cleaning' due to malformed input)
    );
  
    // Ensure that console.log was not called (because no errors occurred)
    expect(console.log).not.toHaveBeenCalled();
  
    // Ensure the operation's state has been cleared after the operation
    expect(component.requireSpecialistChangeHelper).toBe('');
    expect(component.operationTypeResult).toBeNull();
  });*/

