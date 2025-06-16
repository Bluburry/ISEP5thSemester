describe('Login Page Tests', () => {
  beforeEach(() => {
    // Visit the login page before each test
    cy.visit('http://localhost:4200');
  });

  it('should display the login page with all required elements', () => {
    // Check if the title exists
    cy.get('h2').should('contain', 'Login');

    // Check if the username and password input fields exist
    cy.get('#username').should('exist').and('have.attr', 'placeholder', 'Enter username');
    cy.get('#password').should('exist').and('have.attr', 'placeholder', 'Enter password');

    // Check if the login button exists
    cy.get('.login-button').should('exist').and('contain', 'Login');

    // Check if the Google login button exists
    cy.get('.google-button').should('exist').and('contain', 'Google Login');

    // Check if the register button exists
    cy.get('.register-button').should('exist').and('contain', 'Register Account');

    // Check if the error message element exists
    cy.get('.error-message').should('exist');
  });

  it('should display an error message for invalid login credentials', () => {
    // Capture the initial URL
    cy.url().then((initialUrl) => {
    // Input invalid username and password
    cy.get('#username').type('invalidUser');
    cy.get('#password').type('invalidPass');
    
    // Click the login button
    cy.get('.login-button').click();

    // Simulate the error response (mock the backend or handle the expected error)
    cy.intercept('POST', '/api/login', {
      statusCode: 401,
      body: { message: 'Invalid username or password' },
    });

    cy.window().then((win) => {
      expect(win.localStorage.getItem('authToken')).to.be.null;  // Expect null (no authToken)
    });

    // Assert that the URL remains the same
    cy.url().should('eq', initialUrl);
    });
  });

  it('should log in successfully with valid credentials', () => {
    // Fill in valid credentials
    cy.get('input#username').type('testuser');
    cy.get('input#password').type('correctpassword');
    
    // Intercept the login API call and mock a successful response
    cy.intercept('POST', '**/Login', {
      statusCode: 200,
      body: { Token: 'dummyToken', Type: 'ADMIN_AUTH_TOKEN' },
    });

    // Submit the form
    cy.get('button.login-button').click();

    // Verify redirection or the token being stored in localStorage
    cy.window().then((win) => {
      expect(win.localStorage.getItem('authToken')).to.eq('dummyToken');
    });

    // Assert that the correct route is navigated to
    cy.url().should('include', '/adminPanel');
  });
});
