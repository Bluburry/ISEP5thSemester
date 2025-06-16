/// <reference types="cypress" />

describe("Admin create Specialization", () => {
	beforeEach(() => {
		// Mock API responses using Cypress intercept
		cy.intercept({
			method: 'POST',
			url: 'https://localhost:5001/api/Tokens'
		}).as('validateToken');
		cy.intercept({
			method: 'GET',
			url: 'https://localhost:5001/api/Specialization/GetSpecializationList'
		}).as('getSpecializations');
		cy.intercept({
			method: 'DELETE',
			url: 'https://localhost:5001/api/Specialization/DeleteSpecialization/*'
		}).as('deleteSpecialization');
		cy.intercept({
			method: 'PATCH',
			url: 'https://localhost:5001/api/Specialization/EditSpecialization/*'
		}).as('patchSpecialization');

		localStorage.setItem('authToken', 'c6dcb583-c2e6-4893-966b-9a5f68c9b88a');
		cy.visit('http://localhost:4200/specialization-control');
	});

	it('redirects unauthenticated users to login page', () => {
		localStorage.removeItem('authToken');
		cy.visit('http://localhost:4200/doctor-operation-control');
		cy.url().should('eq', 'http://localhost:4200/');
	});

	it('loads patients and operation types on initialization', () => {
		cy.wait('@getSpecializations');

		// Assert patients are listed in the scroll box
		cy.get('.scroll-box').should('have.length.greaterThan', 0);
	});

	it('fetches and displays patient details on click', () => {
		cy.window().then((window) => {
			const component = window.ng.getComponent(window.document.querySelector('app-specialization-control'));

			component.initializeData('c6dcb583-c2e6-4893-966b-9a5f68c9b88a');
			component.spDisplay = [{
				SpecializationCode: "testCode1",
				SpecializationName: "testName1",
				SpecializationDescription: "description 1"
			}, {
				SpecializationCode: "testCode2",
				SpecializationName: "testName2",
				SpecializationDescription: "description 2"
			}, {
				SpecializationCode: "testCode3",
				SpecializationName: "testName3",
				SpecializationDescription: "description 3"
			}];

			component.spPicked = component.spDisplay[0];
		});

		// Verify patient details are displayed in the form
		cy.get('#input1').should('not.have.value', '');
		cy.get('.editable-box').should('be.visible');
	});

	it('sends delete request', () => {
		cy.window().then((window) => {
			const component = window.ng.getComponent(window.document.querySelector('app-specialization-control'));

			component.initializeData('c6dcb583-c2e6-4893-966b-9a5f68c9b88a');
			component.spDisplay = [{
				SpecializationCode: "testCode1",
				SpecializationName: "testName1",
				SpecializationDescription: "description 1"
			}, {
				SpecializationCode: "testCode2",
				SpecializationName: "testName2",
				SpecializationDescription: "description 2"
			}, {
				SpecializationCode: "testCode3",
				SpecializationName: "testName3",
				SpecializationDescription: "description 3"
			}];

			component.spPicked = {
				SpecializationCode: "testCode1",
				SpecializationName: "testName1",
				SpecializationDescription: "new description 1"
			};
		});

		cy.get('.update').click();
		cy.wait('@patchSpecialization').its('request.body').should('exist');
	});

	it('sends update request', () => {
		cy.window().then((window) => {
			const component = window.ng.getComponent(window.document.querySelector('app-specialization-control'));

			component.initializeData('c6dcb583-c2e6-4893-966b-9a5f68c9b88a');
			component.spDisplay = [{
				SpecializationCode: "testCode1",
				SpecializationName: "testName1",
				SpecializationDescription: "description 1"
			}, {
				SpecializationCode: "testCode2",
				SpecializationName: "testName2",
				SpecializationDescription: "description 2"
			}, {
				SpecializationCode: "testCode3",
				SpecializationName: "testName3",
				SpecializationDescription: "description 3"
			}];

			component.spPicked = component.spDisplay[0];
		});

		cy.get('.delete').click();
		cy.wait('@deleteSpecialization').its('request.body').should('exist');
	});
});