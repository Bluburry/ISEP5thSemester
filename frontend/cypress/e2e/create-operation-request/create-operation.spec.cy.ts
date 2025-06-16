/// <reference types="cypress" />

describe('Doctor Create Operation Request E2E Tests', () => {
    beforeEach(() => {
        // Mock API responses using Cypress intercept
        cy.intercept({
            method: 'POST',
            url: 'https://localhost:5001/api/Tokens'
        }).as('validateToken');
        cy.intercept({
            method: 'GET',
            url: 'https://localhost:5001/api/OperationRequest?*'
        }).as('getOperationRequest');
        cy.intercept({
            method: 'GET',
            url: 'https://localhost:5001/api/OperationRequest/Operation*'
        }).as('getOperationById');
        cy.intercept({
            method: 'PATCH',
            url: 'https://localhost:5001/api/OperationRequest?*'
        }).as('editOperationRequest');

        localStorage.setItem('authToken', 'a1f925e6-9f86-4d31-ac8f-15762d1297d5');
        cy.visit('http://localhost:4200/doctor-operation-control');
    });

    it('redirects unauthenticated users to login page', () => {
        localStorage.removeItem('authToken');
        cy.visit('http://localhost:4200/doctor-operation-control');
        cy.url().should('eq', 'http://localhost:4200/');
    });

    it('loads operation requests on initialization', () => {
        cy.wait('@getOperationRequest');

        // Assert patients are listed in the scroll box
        cy.get('.scroll-box p').should('have.length.greaterThan', 0);
    });

    it('fetches and displays request details on click', () => {
        cy.wait('@getOperationRequest');
        cy.get('.scroll-box p').first().click();

        cy.get('#input1').should('not.have.value', '');
        cy.get('.editable-box').should('be.visible');
    });

    it('displays operation types', () => {
        cy.wait('@getOperationRequest');
        cy.get('.scroll-box p').first().click();

        cy.wait('@getOperationById');
        cy.get('#input2').should('be.visible');
        cy.get('#input2').should('have.length.greaterThan', 0);
    });

    it('validates and submits the operation request', () => {
        cy.wait('@getOperationRequest');
        cy.get('.scroll-box p').first().click();

        cy.wait('@getOperationById');
        cy.get('#input5') 
		.type('2024-12-25')
		.should('have.value', '2024-12-25'); 

        cy.get('#input6').select('LOW');

        // Submit the form
        cy.get('.update').click();

        // Verify the request was sent
        //cy.wait('@editOperationRequest').its('request.body').should('exist');
    });

    it('shows errors for incomplete form submissions', () => {
        cy.wait('@getOperationRequest');
        cy.get('.scroll-box p').first().click();

        cy.get('.update').click();

        cy.get('@editOperationRequest.all').should('have.length', 0);
    });
});