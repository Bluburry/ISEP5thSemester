describe('Doctor Schedule Page', () => {
  const baseUrl = 'http://localhost:4200/doctor-schedule'; // Change this to your app's running URL
  
  beforeEach(() => {
      window.localStorage.setItem('authToken', 'iJYyeyRwXHbzIDT52aRrZkwpk+JvPjwoX6Mxfwj3vs09cbrjQoQf1YYB+h8qIqWIX3B1ndlpvQKop/A04l3nrr3JBfmpFCMBomCU+NyUBRnfqnLJFAb89ysp1mn5F35cuYcy0qppszc3STWr2UddJWf3LIWoFMQHNS5L2rJ9jiZZAD9Lg25zxPYS+p6awHOMhMUSPATQRm2Lz/NqHDtfA9Z1L5H8aT/RSdcrjROzfqIho26sEyaR0R/J4/ZC0WIBJ+Y2fqlZ4G9jI9eJKYAQ4Brv/gJNfo/SLHLR3hdGNOfuAIRy0rhR3s7TEFYu7JkMd4E41M8jLrbR5OcAlpexXg==');
      cy.visit(baseUrl); // Ensure the application is loaded fresh for each test
  });

  it('should display the logo and navigate to the doctor panel when clicked', () => {
    cy.get('.header nav a')
      .should('have.attr', 'href', '/doctorPanel')
      .find('img')
      .should('have.attr', 'src', '../../../assets/ADMIN-PANEL.png')
      .click();
  });

  it('should select a room, an operation request, and type a date', () => {
    // Step 1: Select a room from the dropdown
    cy.get('select#input1')
      .eq(0) // First dropdown for rooms
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected

    // Step 2: Select an operation request from the dropdown
    cy.get('select#input1')
      .eq(1) // Second dropdown for operation requests
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected

    // Step 3: Type a date
    const mockDate = '2025-01-10 10:00';
    cy.get('input[type="text"]')
      .type(mockDate)
      .should('have.value', mockDate); // Verify the typed date

    // Step 4: Click "Check staff availability" button
    cy.get('.update').click();

    // Step 5: Verify staff list appears
    cy.get('.scroll-box')
      .eq(0) // Assuming staff roster appears in the first scroll-box
      .should('contain', 'Cleaner Hospital - Cleaner') // Verify that a staff member's name is displayed
      .within(() => {
        cy.get('p').should('have.length.greaterThan', 0); // Ensure there are staff entries
      });
  });

  it('should click on a staff member and update the needed specialist count', () => {
    // Step 1: Select a room from the dropdown
    cy.get('select#input1')
      .eq(0) // First dropdown for rooms
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected
  
    // Step 2: Select an operation request from the dropdown
    cy.get('select#input1')
      .eq(1) // Second dropdown for operation requests
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected
  
    // Step 3: Type a date
    const mockDate = '2025-01-10 10:00';
    cy.get('input[type="text"]')
      .type(mockDate)
      .should('have.value', mockDate); // Verify the typed date
  
    // Step 4: Click "Check staff availability" button
    cy.get('.update').click();
  
    // Step 5: Verify staff list appears and click on a specific staff member
    cy.get('.scroll-box')
      .eq(0) // Assuming staff roster appears in the first scroll-box
      .should('contain', 'Cleaner Hospital - Cleaner') // Verify that a specific staff member is displayed
      .within(() => {
        cy.get('p')
          .contains('Cleaner Hospital - Cleaner') // Locate the specific staff entry
          .click(); // Simulate clicking the staff member
      });
  
    // Step 6: Verify that the "Needed Cleaner" count updates to 1/1
    cy.get('.right .input-box')
      .should('contain', 'Needed Cleaner(s): 1 / 1'); // Check if the count is updated
  });

  it('should create an appointment and display the success message', () => {
    // Step 1: Select a room from the dropdown
    cy.get('select#input1')
      .eq(0) // First dropdown for rooms
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected
  
    // Step 2: Select an operation request from the dropdown
    cy.get('select#input1')
      .eq(1) // Second dropdown for operation requests
      .select(1) // Select the second option (index 1, because index 0 is disabled)
      .should('not.have.value', ''); // Ensure a value is selected
  
    // Step 3: Type a date
    const mockDate = '2025-01-10 10:00';
    cy.get('input[type="text"]')
      .type(mockDate)
      .should('have.value', mockDate); // Verify the typed date
  
    // Step 4: Click "Check staff availability" button and assign a specialist
    cy.get('.update').click();
  
    cy.get('.scroll-box')
      .eq(0) // Assuming staff roster appears in the first scroll-box
      .should('contain', 'Cleaner Hospital - Cleaner') // Verify that a specific staff member is displayed
      .within(() => {
        cy.get('p')
          .contains('Cleaner Hospital - Cleaner')
          .click();
        cy.get('p')
          .contains('John Bottles - Doctor') // Locate the specific staff entry
          .click(); // Simulate clicking the staff member
        cy.get('p')
          .contains('Anaesthesist Hospital - Anaesthesist') // Locate the specific staff entry
          .click(); // Simulate clicking the staff member
      });
  
    // Step 5: Click "Schedule appointment" button
    cy.get('.add').click();
  
    // Step 6: Verify the success message is displayed
    cy.get('.right')
      .should('contain', 'Appointment successfully created'); // Check if the success message is displayed
  });
});