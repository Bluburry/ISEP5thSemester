# Team Planning Decisions

In this file, the team registers decisions made during meetings and records interactions that may affect the content of the project, for example: shared feedback, shift of work focus and help shared between peers.

## Project Decisions

### Software Development Approach

There are many possible software development approaches that the team could choose, since non were demanded to follow.

The team decided to follow the **Domain Driven Development**, since this was the one most confortable to every member. This meant that the **Tests** portion in the documentation change spots from the **Design** Section to the **Implementation** Section, because the tests are now made *after* the implementation, and not *before*, like in the **Test Driven Development**.

### Definition of Done (Task Completed)

The items marked with the status "Task Completed" have been fully completed.
The "Done" status is being used to define when the tasked member has completed the User Story, but the new implementation hasn't been tested by a different member.

Some User Stories have child issues, that follow our defined set of phases. For those issues, we consider them done when:

- **Documentation:** All the 6 documentation topics have been written and completed. The proof-reading task (*Task: Documentation Manage*) **does not** prevent the User Story for being considered done.
  Each topic is completed when:
  - **Context/Requirements:** All client clarifications, acceptance criteria and dependencies are written.
  - **Analysis:** All the studied and discussed plan for the design is documented with the necessary artifacts *(team-defined-standard: SSD & Domain Model)*.
  - **Design:** The sequence diagram is completly done, following the studied plan in the analysis section.
  - **Implementation:** The design choices have been fully implemented, and the functionality is operational.
  - **Demonstration:** All the required proof that the functionality is working is provided.
- **Implementation:** The functionality is implemented and  fully tested (this includes own testing and other member's testing), not needing anymore updates during that sprint.
- **Test Creation:** All the automated tests are created and ready to be demonstrated.

The existence of these issues depend on the group element's preference.

### Platform User Authentication Process

Considering the user authentication and authorization process of the system is performed on most user stories this process was opted to be generalized to prevent documentation redundancy.
As is presented in the following Level 3 Process View, represented in the form of a generalized Sequence Diagram that demonstrates the flow of token inspection to ensure proper user authorization and authentication.

![AuthUser](/docs/global-artifacts/SD/AuthUser.svg)

### Platform Object Logging Process

The team arrived to the conclusion that since so many user stories require the logging of objects and other types of information that we would be better off generalizing the process to avoid redundancy.

A controller will call a method in the LogService class dedicated specifically to handling the logging of a specific instance of data, this method will then send the data to be logged to the AddAsync method in the same LogService in the form of a LogDto that can be easily handled and persisted in the database.

![Log-Object-SD](/docs/global-artifacts/SD/Log-Object-SD.svg)

### Non-specified Object Restrictions

There are many attributes for certain objects that don't present a specific restriction on the project description and that are too insignificant to ask the client.
This decisions will be recorded here to make it easier to know which restrictions were made and what needs to be changed.

### Client Clarifications - Questions Format

Since the questions made to the client aren't User Story specific, the team decided to write each question and answer following the format:

>[**"QUESTION"** *by PERSON_NAME NUMBER(1220000) - WEEK_DAY, DATE(extended) at HOUR*]
>*question*
>> Answer - *answer*.

## GDPR Compliance Team Decisions

The team promises to comply with GDPR regulation when handling patient data:

1. **Data Processing Principles:** The principles include purpose limitation, data minimization, accuracy, storage limitation, integrity, and confidentiality. These core principles guide data handling to ensure personal data is only processed as needed, accurately, and securely.

2. **Data Security:** Security obligations for controllers and processors are emphasized. Measures must ensure data confidentiality, integrity, and resilience of processing systems. Regular testing and assessment of security measures are recommended to prevent unauthorized access, data loss, or alteration.

3. **Accountability and Compliance:** Controllers are responsible for proving compliance with GDPR, implementing appropriate technical and organizational measures, and maintaining detailed records of processing activities.

4. **Data Subject Rights:** Clear rights are outlined for individuals, including the right to access, rectify, and delete personal data. Data subjects must also be informed about data processing activities in a transparent and understandable way.

5. **Risk Assessment and Mitigation:** The GDPR encourages the use of data protection impact assessments, especially for high-risk processing activities, to proactively identify and mitigate potential data protection risks.

### Patient Sensitive Data

Patient sensitive data is identified as uniquely identifying information regarding any one user of the platform this is defined more specifically as:

>**Article 4, Section 1 of Official EU GDPR Law:** "Personal data" refers to information relating to an identified or identifiable natural person ("data subject"). A natural person is considered identifiable if they can be identified, directly or indirectly, particularly by reference to an identifier such as a name, identification number, location data, electronic identifiers, or one or more specific elements of their physical, physiological, genetic, mental, economic, cultural, or social identity.

>**Articles 17, 23 and 30 of Official EU GDPR Law:** Personal data should be retained only for as long as necessary to fulfill the purpose for which it was collected, after which it should be deleted. If the purpose of the data has been completed, or if the data subject withdraws their consent or exercises their right to erasure, the data controller must ensure its removal. Data controllers are responsible for setting appropriate retention periods based on the nature and purpose of the data processing to ensure compliance with data minimization and purpose limitation principles.

#### Considering the information gathered and the dialogue between the team the following was decided:

- When deleting a user off the system they must be either delete completely off the system or have their **personally identifying information** completely anonymized beyond possible recognition
- Upon a deletion request of user data the team is by legal agreement allowed to withhold the user's information for at least **1 week (7 days)** before deletion to prevent critical system disruption events.

## Meetings

## Meetings

> **[27/11/2024 18:00]**
> The team reviewed the initial tasks for Sprint 3 and assigned ownership of each User Story.

> **[05/12/2024 18:00]**
> Conducted a sprint retrospective for Sprint 2. Noted that while all the tasks were sucessfull, the project focus should be also be directed to the user experience, to better sell the product to the client.

> **[13/12/2024 18:00]**
> The team discussed client clarifications and agreed on how to proceed with the undocumented requests.

> **[20/12/2024 18:00]**
> Mid sprint check-in to evaluate progress on Sprint 3 tasks. Identified challenges in integrating the some new processes with existing components (the new backend, new medicalCondition + allergies, ...). Discussed the best solutions for them.

> **[05/01/2025 18:00]**
> Held a wrap-up meeting for Sprint 3. Reviewed all completed tasks and ensured the documentation and tests met the Definition of Done.
