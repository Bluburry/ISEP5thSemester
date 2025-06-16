/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import expressLoader from './express';
import dependencyInjectorLoader from './dependencyInjector';
import mongooseLoader from './mongoose';
import Logger from './logger';

import config from '../../config';
import AllergyController from '../controllers/allergyController';
import AllergySchema from '../persistence/schemas/AllergySchema';

export default async ({ expressApp }) => {
  const mongoConnection = await mongooseLoader();
  Logger.info('✌️ DB loaded and connected!');

  const userSchema = {
    // compare with the approach followed in repos and services
    name: 'userSchema',
    schema: '../persistence/schemas/userSchema',
  };

  const roleSchema = {
    // compare with the approach followed in repos and services
    name: 'roleSchema',
    schema: '../persistence/schemas/roleSchema',
  };

  const allergySchema = {
    // compare with the approach followed in repos and services
    name: 'allergySchema',
    schema: '../persistence/schemas/AllergySchema',
  };

  const conditionSchema = {
    // compare with the approach followed in repos and services
    name: 'conditionSchema',
    schema: '../persistence/schemas/MedicalConditionSchema',
  };

  const clinicalDetailsSchema = {
    // compare with the approach followed in repos and services
    name: 'clinicalDetailsSchema',
    schema: '../persistence/schemas/ClinicalDetailsSchema',
  };
  const roleController = {
    name: config.controllers.role.name,
    path: config.controllers.role.path
  }

  const allergyController = {
    name: config.controllers.allergy.name,
    path: config.controllers.allergy.path
  }

  const medicalCondtionController = {
    name: config.controllers.condition.name,
    path: config.controllers.condition.path
  }


  const clinicalDetailsController = {
    name: config.controllers.clinicalDetails.name,
    path: config.controllers.clinicalDetails.path
  }

  const roleRepo = {
    name: config.repos.role.name,
    path: config.repos.role.path
  }

  const userRepo = {
    name: config.repos.user.name,
    path: config.repos.user.path
  }

  const allergyRepo = {
    name: config.repos.allergy.name,
    path: config.repos.allergy.path
  }

  const conditionRepo = {
    name: config.repos.conditions.name,
    path: config.repos.conditions.path
  }

  const clinicalDetailsRepo = {
    name: config.repos.clinicalDetails.name,
    path: config.repos.clinicalDetails.path
  }

  const roleService = {
    name: config.services.role.name,
    path: config.services.role.path
  }

  const allergyService = {
    name: config.services.allergy.name,
    path: config.services.allergy.path
  }

  const conditionService = {
    name: config.services.condition.name,
    path: config.services.condition.path
  }
  
  const clinicalDetailsService = {
    name: config.services.clinicalDetails.name,
    path: config.services.clinicalDetails.path
  }

  await dependencyInjectorLoader({
    mongoConnection,
    schemas: [
      userSchema,
      roleSchema,
      allergySchema,
      conditionSchema,
      clinicalDetailsSchema
    ],
    // eslint-disable-next-line prettier/prettier
    controllers: [
      roleController,
      allergyController,
      medicalCondtionController,
      clinicalDetailsController
    ],
    // eslint-disable-next-line prettier/prettier
    repos: [
      roleRepo,
      userRepo,
      allergyRepo,
      conditionRepo,
      clinicalDetailsRepo
    ],
    services: [
      roleService,
      allergyService,
      conditionService,
      clinicalDetailsService
    ]
  });
  Logger.info('✌️ Schemas, Controllers, Repositories, Services, etc. loaded');

  await expressLoader({ app: expressApp });
  Logger.info('✌️ Express loaded');
};
