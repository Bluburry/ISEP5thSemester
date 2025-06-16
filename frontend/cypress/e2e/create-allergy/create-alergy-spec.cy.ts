/// <reference types="cypress" />

describe('Allergy Creation Tests', () => {
    const baseUrl = 'http://localhost:4200/admin-allergy-control'; // Change this to your app's running URL
  
    beforeEach(() => {
        window.localStorage.setItem('authToken', 'gTzTzlLJWE2plqap4wKCucSO25he8wX7xyvOzImS41orfq+7cMsvG+X3bv40JekHZorHEZ3jJhcX1uTH/1x7+3vpGGVMj695H6TgNNt98T4M5VqwQnLdTLZJ3kG0Cz02H380b33BHMYAFKd9ZZxr5+JZ1kyzyU5oCTA5bIU8HT76uX1vR5W5INNh5H3nSQ1G99nCzOCqThZ4zl3CNY6Ypjch7G7OjupMOlWlOPTnAXjnFzq+31+iEJUc55QLL6ZUExOYOWhSb58+KhC0UsyGvf65ZfoyFsaVZTnuMlJzPON2uJIMIQptucZGjkCj70X4yrxVpya/ySe2q1I2NIxDaQ==');
        cy.visit(baseUrl); // Ensure the application is loaded fresh for each test
    });
  
    it('should display the allergy creation form', () => {
      // Verify that the form and input fields are visible
      cy.get('h3').contains('Create Allergy').should('be.visible');
      cy.get('#createCode').should('be.visible');
      cy.get('#createName').should('be.visible');
      cy.get('#createDescription').should('be.visible');
    });
  
    it('should create an allergy successfully', () => {
      const allergy = {
        id: 'ABCD.1', // or leave empty for auto generation
        name: 'Peanut Allergy',
        description: 'Allergy to peanuts'
      };
  
      // Fill out the form fields
      cy.get('#createCode').type(allergy.id); // or leave empty for auto generation
      cy.get('#createName').type(allergy.name);
      cy.get('#createDescription').type(allergy.description);
  
      // Submit the form
      cy.get('.create').click();
  
      // Verify the success status message
      cy.get('.status-message')
        .should('be.visible')
        .and('contain', 'Allergy created successfully');
    });
  
});
  