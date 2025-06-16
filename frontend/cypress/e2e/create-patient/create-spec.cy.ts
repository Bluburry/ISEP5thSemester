import { LoginResponse } from '../../../src/app/login-result';
describe('Admin Create Patient Panel', () => {
  const validPatientData = {
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'johndoe@example.com',
    phone: '919505991',
    dateOfBirth: '1980-01-01',
    gender: 'Male',
    emergencyContact: '919505891',
  };

  const errorMessage = 'Missing information for:';
  const successMessage = 'Patient Created Sucessfully!';
  
  beforeEach(() => {
    // Set the localStorage auth token before visiting the page
    window.localStorage.setItem('authToken', 'd17a7e65-02c0-4189-affa-3101b4c0a4e9');
    cy.visit('http://localhost:4200/admin-patient-create');
  });

  it('should display the patient form', () => {
    cy.get('input#input1').should('be.visible'); // First Name input
    cy.get('input#input2').should('be.visible'); // Last Name input
    cy.get('button.update').should('be.visible'); // CREATE button
  });

  it('should create a patient successfully', () => {
    // Fill in the form with valid data
    cy.get('input#input1').type(validPatientData.firstName);
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input4').type(validPatientData.email);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input7').type(validPatientData.phone);
    cy.get('input#input8').type(validPatientData.emergencyContact);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if success message is shown
    cy.get('p').contains(successMessage).should('be.visible');
    cy.get('p').contains(errorMessage).should('not.exist');
  });

  it('should display an error if required fields are missing', () => {
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input4').type(validPatientData.email);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input7').type(validPatientData.phone);
    cy.get('input#input8').type(validPatientData.emergencyContact);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if error message is shown
    cy.get('p').contains(errorMessage).should('be.visible');
    cy.get('p').contains(successMessage).should('not.exist');
  });

  it('should display missing firstName if no firstName', () => {
    // Leave out one required field (e.g., first name)
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input4').type(validPatientData.email);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input7').type(validPatientData.phone);
    cy.get('input#input8').type(validPatientData.emergencyContact);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if error message is shown
    cy.get('p').contains(errorMessage).should('be.visible');
    cy.get('p').contains(errorMessage).should('have.text', `Missing information for: firstName`);
    cy.get('p').contains(successMessage).should('not.exist');

    cy.get('p').contains(errorMessage)
  });

  it('should display missing email if no email', () => {
    // Fill in the form with valid data
    cy.get('input#input1').type(validPatientData.firstName);
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input7').type(validPatientData.phone);
    cy.get('input#input8').type(validPatientData.emergencyContact);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if success message is shown
    cy.get('p').contains(errorMessage).should('be.visible');
    cy.get('p').contains(errorMessage).should('have.text', `Missing information for: email`);
    cy.get('p').contains(successMessage).should('not.exist');

    cy.get('p').contains(errorMessage)
  });

  it('should display missing phone if no phone', () => {
    cy.get('input#input1').type(validPatientData.firstName);
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input4').type(validPatientData.email);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input8').type(validPatientData.emergencyContact);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if error message is shown
    cy.get('p').contains(errorMessage).should('be.visible');
    cy.get('p').contains(errorMessage).should('have.text', `Missing information for: phone`);
    cy.get('p').contains(successMessage).should('not.exist');

    cy.get('p').contains(errorMessage)
  });

  it('should display missing emergencyContact if no emergency contact', () => {
    cy.get('input#input1').type(validPatientData.firstName);
    cy.get('input#input2').type(validPatientData.lastName);
    cy.get('input#input3').type(validPatientData.fullName);
    cy.get('input#input4').type(validPatientData.email);
    cy.get('input#input5').type(validPatientData.gender);
    cy.get('input#input6').type(validPatientData.dateOfBirth);
    cy.get('input#input7').type(validPatientData.phone);

    // Click the 'CREATE' button
    cy.get('button.update').click();

    // Check if error message is shown
    cy.get('p').contains(errorMessage).should('be.visible');
    cy.get('p').contains(errorMessage).should('have.text', `Missing information for: emergencyContact`);
    cy.get('p').contains(successMessage).should('not.exist');

    cy.get('p').contains(errorMessage)
  });

  it('should initialize token on init', () => {
    // Visit the page again to trigger login token assignment
    cy.visit('http://localhost:4200/admin-patient-create');
  
    // Wait for the localStorage to be updated
    cy.window().should('have.property', 'localStorage');
    cy.window().then((window) => {
      
    });
    cy.wrap(window.localStorage.getItem('authToken')).should('exist');
    const token = window.localStorage.getItem('authToken');
    expect(token).to.eq('d17a7e65-02c0-4189-affa-3101b4c0a4e9');
    cy.wrap(window.localStorage.getItem('authToken')).should('exist');

  });

  it('should retrieve the token successfully after initializing', () => {
    cy.get('#response-token').should('have.text', 'd17a7e65-02c0-4189-affa-3101b4c0a4e9');
  });
  
});
