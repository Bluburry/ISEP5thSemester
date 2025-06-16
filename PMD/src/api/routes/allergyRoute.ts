/* eslint-disable prettier/prettier */
import { Router } from 'express';
import { celebrate, Joi } from 'celebrate';

import { Container } from 'typedi';

import AllergyController from '../../controllers/allergyController';

const route = Router();

export default (app: Router) => {
  app.use('/allergies', route);

  const ctrl = Container.get(AllergyController);

  route.post(
    '',
    celebrate({
      body: Joi.object({
        name: Joi.string().required(),
        description: Joi.string().optional(),
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
      ctrl.createAllergy(token, req, res, next);
    },
  );
  

  route.get(
    '',
    celebrate({
      query: Joi.object({
        code: Joi.string().allow('').optional(), // Allow empty strings
        name: Joi.string().allow('').optional(), // Allow empty strings
        description: Joi.string().allow('').optional(), // Allow empty strings
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
  
      // Call the controller method
      ctrl.queryAllergies(token, req, res, next);
    },
  );
  

  route.patch(
    '',
    celebrate({
      body: Joi.object({
        code: Joi.string().optional(),
        name: Joi.string().optional(),
        description: Joi.string().optional(),
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
      const id = req.headers['icd'] as string;
      console.log(id);
      // Call the controller method
      ctrl.patchAllergies(token, id, req, res, next);
    },
  );
};
