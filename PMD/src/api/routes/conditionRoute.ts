/* eslint-disable prettier/prettier */
import { Router } from 'express';
import { celebrate, Joi } from 'celebrate';

import { Container } from 'typedi';

import MedicalConditionController from '../../controllers/medicalConditionController';

const route = Router();

export default (app: Router) => {
  // Bind the route for medical conditions
  app.use('/medical-conditions', route);

  const ctrl = Container.get(MedicalConditionController);

  // Route for creating a medical condition
  route.post(
    '/',
    celebrate({
      body: Joi.object({
        code: Joi.number().required(),
        designation: Joi.string().required(),
        description: Joi.string().optional().allow(null),
        symptoms: Joi.string().required(),
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
      ctrl.createCondition(token, req, res, next);
    }
  );

  // Route for searching medical conditions
  route.get(
    '/',
    celebrate({
      query: Joi.object({
        code: Joi.string().optional().allow(''), // Optional query parameter
        designation: Joi.string().optional().allow(''),
        description: Joi.string().optional().allow(''),
        symptoms: Joi.string().optional().allow(''),
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
      ctrl.searchCondition(token, req, res, next);
    }
  );
};
