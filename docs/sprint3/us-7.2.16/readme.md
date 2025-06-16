# 7.2.16 As Admin I want to update an allergy

## 1. Context

This is the first time this US is tasked to us. It tasks the implementation of a new functionality that requires the update of the previously implemented objects.

This functionality adds a new complexity to the patient profile that will be controlled by the admin.

## 2. Requirements

"**US 7.2.16 -** As Admin I want to update an allergy

**Acceptance Criteria:**

- **US 7.2.4.1** The user should be able to edit the allergy's designation and description


**Dependencies/References:**

- "**US 7.2.3** - *As a Doctor, I want to search for Allergies, so that I can use it to update the Patient Medical Record.*"
- The US 7.2.1 needs to be completed before the rest of the Patient Profile related USs.
- "**US 7.2.6** - *As a Doctor, I want to update the Patient Medical Record, namely respecting Medical Conditions and Allergies.*"
- "**US 7.2.7** - *As a Doctor, I want to search for entries in the Patient Medical Record, namely respecting Medical Conditions and Allergies.*"

## 3. Analysis

The functionality is rather simple in concept, just aims to facilitate the storage of allergy records in the database of the PMD backend, this will however require some refactoring of the Backoffice module and have some things moved from it to the new PMD module, that will be where the brunt of the work is.

Previously, the MedicalConditions/Allergies was a single attribute with no required data, being merely a string. Now, they are two different attributes and the Medical Conditions needs to have the following information:

- Code (IDC)
- Designation
- (Optional) Longer Description

### Relevant DM Excerpts

![Domain Model Excerpt](DM-excerpt.svg "Domain Model Excerpt")

### System Sequence Diagram

![SSD](SSD.svg "")

## 4. Design

### 4.1. Realization

![Sequence Diagram](SD.svg "")

### 4.2. Applied Patterns

- Aggregate
- Entity
- Value Object
- Service
- MVC
- Layered Architecture
- DTO
- Clean Architecture
- C4+1

### 4.3. Design Commits

> **01/11/2024 10:00 [US6.0.0]** (...)
>



## 5.1. Code Implementation

## Frontend

### Component

[Component](../../../frontend/src/app/Admin/allergy-control/allergy-controller/allergy-controller.component.css)

### Service

[Admin Service](../../../frontend/src/app/Admin/admin.service.ts)

### Controller

[Allergy Controller](../../../PMD/src/controllers/allergyController.ts)

### Service

[Allergy Service](../../../PMD/src/services/allergyService.ts)

### Repository

[Allergy Repostiory](../../../PMD/src/repos/allergyRepo.ts)

### Utilities

[AllergyDTO](../../../PMD/src/dto/IAllergyDTO.ts)
[Allergy](../../../PMD/src/domain/Allergy.ts)
[AllergyId](../../../PMD/src/domain/AllergyId.ts)
[AllergyMap](../../../PMD/src/mappers/AllergyMap.ts)

## 5.2. Tests

**Assigned Tester:** Ricardo Dias

## Unit Tests

This section provides an overview of the unit tests for the `allergy-controller.component` (front-end).

**Test File:** [allergy-controller.component.spec.cs](../../..frontend/src/app/Admin/allergy-control/allergy-controller/allergy-controller.component.spec.ts)

### Test Cases

1. **All the Tests**
   - should create the component
   - should initialize token from localStorage on ngOnInit
   - should call getAllergies and set allergies correctly
   - should set status message when getAllergies fails
   - should call patchAllergies and update the status message
   - should set status message when patchAllergies fails
   - should call createAllergy and set success status message
   - should set status message when createAllergy fails
   - should reset queryData after creating an allergy
   - should set selectedAllergy when fetchAllergyById is called
   - should set selectedAllergy to null when fetchAllergyById does not find the allergy

This section provides an overview of the unit tests for the `AllergyController`.

**Test File:** [allergyController.spec.cs](../../../PMD/tests/unit/controller/allergyController.spec.ts)

### Test Cases

1. **patchAllergies**
   - should return 400 if token is missing
   - should return 401 if token is unauthorized
   - should return 400 if service fails
   - should return 201 if service succeeds

This section provides an overview of the unit tests for the `AllergyService`.

**Test File:** [allergyService.spec.cs](../../../PMD/tests/unit/services/allergyService.spec.ts)

### Test Cases

1. **patchAllergies**
   - should return 400 if patch fails
   - should return 200 if allergy is patched successfully

### US7.2.2 Integration Tests

This section provides an overview of the integration tests for the `allergyIntegration` class. These tests ensure proper integration between controller and service layers.

**Test File:** [allergyIntegration.spec.cs](../../../PMD/tests/integration/allergyIntegration.spec.ts)

#### Test Cases

1. **patchAllergies**
   - should return 200 and the updated allergy if token is valid
   - should return 400 if allergy does not exist
   - should return 500 if there is an error in the service

### System/E2E Testing

> Performed through POSTMAN, the modules through which system testing was done can be accessed in the following file:
>>[System Testing](../../../backoffice/test/SystemTest/Allergies-Testing.postman_collection.json)

### Main Commits

> **31/12/2024 19:21 [documentation update/fix (forgot DM excerpts my bad)]**
> Alfredo Augusto da Silva Ferreira

> **30/12/2024 18:02 [implementation documentation update]**
> Alfredo Augusto da Silva Ferreira

> **11/12/2024 09:30 [implementation first iteration]**
> Alfredo Augusto da Silva Ferreira

> **11/12/2024 08:53 [documentation]**
> Alfredo Augusto da Silva Ferreira

## 6. Integration/Demonstration

![Demonstration](demo.png)

## 7. Observations

(...)
