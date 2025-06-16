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
			url: 'https://localhost:5001/api/Patient'
		}).as('getPatients');
		cy.intercept({
			method: 'GET',
			url: 'https://localhost:5001/api/Patient/id*'
		}).as('getPatientByID');
		cy.intercept({
			method: 'Get',
			url: 'https://localhost:5001/api/OperationType/ListForOperation'
		}).as('getOperationTypes');
		cy.intercept({
			method: 'POST',
			url: 'https://localhost:5001/api/OperationRequest?*'
		}).as('createOperationRequest');

		localStorage.setItem('authToken', 'c6dcb583-c2e6-4893-966b-9a5f68c9b88a');
		cy.visit('http://localhost:4200/doctor-operation-request');
	});

	it('redirects unauthenticated users to login page', () => {
		localStorage.removeItem('authToken');
		cy.visit('http://localhost:4200/doctor-operation-request');
		cy.url().should('eq', 'http://localhost:4200/');
	});

	it('loads patients and operation types on initialization', () => {
		cy.wait('@getPatients');
		cy.wait('@getOperationTypes');

		// Assert patients are listed in the scroll box
		cy.get('.scroll-box p').should('have.length.greaterThan', 0);
	});

	it('fetches and displays patient details on click', () => {
		cy.wait('@getPatients');
		cy.get('.scroll-box p').first().click();

		// Verify patient details are displayed in the form
		cy.get('#input1').should('not.have.value', '');
		cy.get('.editable-box').should('be.visible');
	});

	it('displays operation types', () => {
		cy.wait('@getPatients');
		cy.wait('@getOperationTypes');
		cy.get('.scroll-box p').first().click();

		cy.wait('@getPatientByID');
		cy.get('#input2').should('be.visible');
		cy.get('#input2').should('have.length.greaterThan', 0);
	});

	it('validates and submits the operation request', () => {
		cy.wait('@getPatients');
		cy.wait('@getOperationTypes');
		cy.get('.scroll-box p').first().click();

		cy.wait('@getPatientByID');
		// Fill in the form
		cy.get('#input2').select('heartSurgery');
		cy.get('#input3').type('2024-12-31'); // Deadline
		cy.get('#input4').select('1');

		// Submit the form
		cy.get('.update').click();

		// Verify the request was sent
		cy.wait('@createOperationRequest').its('request.body').should('exist');
	});

	it('shows errors for incomplete form submissions', () => {
		cy.wait('@getPatients');
		cy.get('.scroll-box p').first().click();

		cy.get('.update').click();

		cy.get('@createOperationRequest.all').should('have.length', 0);
	});
});
