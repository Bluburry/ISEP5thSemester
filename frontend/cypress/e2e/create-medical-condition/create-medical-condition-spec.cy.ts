/// <reference types="cypress" />

describe('Medical Condition Controller E2E Tests', () => {
    const baseUrl = 'http://localhost:4200/admin-condition-control'; // Change this to your app's running URL
  
    beforeEach(() => {
        window.localStorage.setItem('authToken', 'gTzTzlLJWE2plqap4wKCucSO25he8wX7xyvOzImS41orfq+7cMsvG+X3bv40JekHZorHEZ3jJhcX1uTH/1x7+3vpGGVMj695H6TgNNt98T4M5VqwQnLdTLZJ3kG0Cz02H380b33BHMYAFKd9ZZxr5+JZ1kyzyU5oCTA5bIU8HT76uX1vR5W5INNh5H3nSQ1G99nCzOCqThZ4zl3CNY6Ypjch7G7OjupMOlWlOPTnAXjnFzq+31+iEJUc55QLL6ZUExOYOWhSb58+KhC0UsyGvf65ZfoyFsaVZTnuMlJzPON2uJIMIQptucZGjkCj70X4yrxVpya/ySe2q1I2NIxDaQ==');
        cy.visit(baseUrl); // Ensure the application is loaded fresh for each test
    });
  
    it('Should create a new medical condition and check if it is added to the list', () => {
      // Fill out the form
      cy.get('#createCode').type('123456789');
      cy.get('#createDesignation').type('Asthma');
      cy.get('#createSymptoms').type('Shortness of breath');
      cy.get('#createDescription').type('Horrible');
  
      // Click the create button
      cy.get('.create').click();
  
      // Check success message
      cy.contains('Medical Condition created successfully').should('be.visible');
  
      
    });

    it('Created Medical Condition should show in Scroll Box after Creation', () => {
        cy.get('.scroll-box')
      .should('contain.text', '123456789')
      .and('contain.text', 'Asthma')
      .and('contain.text', 'Horrible');
    });
  
    it('Should show error message if mandatory fields are empty', () => {
      // Leave form fields empty
      cy.get('#createCode').clear();
      cy.get('#createDesignation').clear();
      cy.get('#createSymptoms').clear();
      cy.get('#createDescription').clear();
  
      // Click the create button
      cy.get('.create').click();
  
      // Check error message
      cy.contains('Error creating Medical Condition: All the spaces which aren\'t optional need to be filled.')
        .should('be.visible');
    });

    it('Should show error message if code is empty', () => {
        // Leave form fields empty
        cy.get('#createCode').clear();
        cy.get('#createDesignation').type('Asthma');
        cy.get('#createSymptoms').type('Shortness of breath');
        cy.get('#createDescription').type('Horrible');

        // Click the create button
        cy.get('.create').click();
    
        // Check error message
        cy.contains('Error creating Medical Condition: All the spaces which aren\'t optional need to be filled.')
          .should('be.visible');
      });

      it('Should show error message if designation is empty', () => {
        // Leave form fields empty
        cy.get('#createCode').type('123456789')
        cy.get('#createDesignation').clear();
        cy.get('#createSymptoms').type('Shortness of breath');
        cy.get('#createDescription').type('Horrible');

        // Click the create button
        cy.get('.create').click();
    
        // Check error message
        cy.contains('Error creating Medical Condition: All the spaces which aren\'t optional need to be filled.')
          .should('be.visible');
      });

      it('Should show error message if symptoms is empty', () => {
        // Leave form fields empty
        cy.get('#createCode').type('123456789')
        cy.get('#createDesignation').type('Asthma');
        cy.get('#createSymptoms').clear();
        cy.get('#createDescription').type('Horrible');

        // Click the create button
        cy.get('.create').click();
    
        // Check error message
        cy.contains('Error creating Medical Condition: All the spaces which aren\'t optional need to be filled.')
          .should('be.visible');
      });


      it('Should allow creation if  only optional field is empty', () => {
        // Leave form fields empty
        cy.get('#createCode').type('1234512789')
        cy.get('#createDesignation').type('Pneumothorax');
        cy.get('#createSymptoms').type('Shortness of breath');
        cy.get('#createDescription').clear();

        // Click the create button
        cy.get('.create').click();
  
        // Check success message
        cy.contains('Medical Condition created successfully').should('be.visible');
      });


      it('Should display error message for code format mismatch', () => {
        // Leave form fields empty
        cy.get('#createCode').type('112')
        cy.get('#createDesignation').type('Pneumothorax');
        cy.get('#createSymptoms').type('Shortness of breath');
        cy.get('#createDescription').clear();

        // Click the create button
        cy.get('.create').click();
        
        // Check success message
        cy.contains('Error creating Medical Condition: Code must be a number between 6 and 18 digits to be eligible').should('be.visible')
      });
  });
  