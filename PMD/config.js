/* eslint-disable prettier/prettier */
import dotenv from 'dotenv';

// Set the NODE_ENV to 'development' by default
process.env.NODE_ENV = process.env.NODE_ENV || 'development';

const envFound = dotenv.config();
if (!envFound) {
  // This error should crash whole process

  throw new Error("⚠️  Couldn't find .env file  ⚠️");
}

export default {
  /**
   * Your favorite port : optional change to 4000 by JRT
   */
  port: parseInt(process.env.PORT, 10) || 4000, 

  /**
   * That long string from mlab
   */
  databaseURL: process.env.MONGODB_URI || "mongodb://127.0.0.1:27017/test",


  /**
   * Your secret sauce
   */
  jwtSecret: process.env.JWT_SECRET || "nope",

  /**
   * Used by winston logger
   */
  logs: {
    level: process.env.LOG_LEVEL || 'info',
  },

  /**
   * API configs
   */
  api: {
    prefix: '/api',
  },

  controllers: {
    role: {
      name: "RoleController",
      path: "../controllers/roleController"
    },
    allergy: {
      name: "AllergyController",
      path: "../controllers/allergyController"
    },
    condition: {
      name: "MedicalConditionController",
      path: "../controllers/medicalConditionController"
    },
    clinicalDetails: {
      name: "clinicalDetailsController",
      path: "../controllers/ClinicalDetailsController"
    }
  },

  repos: {
    role: {
      name: "RoleRepo",
      path: "../repos/roleRepo"
    },
    user: {
      name: "UserRepo",
      path: "../repos/userRepo"
    },
    allergy: {
      name: "AllergyRepo",
      path: "../repos/allergyRepo"
    },
    conditions: {
      name: "ConditionsRepo",
      path: "../repos/conditionRepo"
    },
    clinicalDetails: {
      name: "ClinicalDetailsRepo",
      path: "../repos/clinicalDetailsRepo"
    }
  },

  services: {
    role: {
      name: "RoleService",
      path: "../services/roleService"
    },
    allergy: {
      name: "AllergyService",
      path: "../services/allergyService"
    },
    condition: {
      name: "ConditionService",
      path: "../services/conditionService"
    },
    clinicalDetails: {
      name: "clinicalDetailsService",
      path: "../services/ClinicalDetailsService"
    }
  },
};
