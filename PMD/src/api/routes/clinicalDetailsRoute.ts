/* eslint-disable prettier/prettier */
import { Router } from 'express';
import { celebrate, Joi } from 'celebrate';
import { Container } from 'typedi';
import ClinicalDetailsController from '../../controllers/ClinicalDetailsController';

const route = Router();

export default (app: Router) => {
  app.use('/clinicalDetails', route);

  const ctrl = Container.get(ClinicalDetailsController);

  // Create a blank clinical detail
  route.post(
    '/blank',
    (req, res, next) => {
      const token = req.headers['token'] as string;
      const mrn = req.headers['mrn'] as string;
      ctrl.createBlankClinicalDetails(token,mrn, res, next);
    }
  );

  route.post(
    '/save',
    celebrate({
      body: Joi.object({
        patientMRN: Joi.string().required(),
        allergies: Joi.array().items(
          Joi.object({
            id: Joi.string().required(),
            name: Joi.string().required(),
            description: Joi.string().optional(),
          })
        ).optional(),
        medicalConditions: Joi.array().items(
          Joi.object({
            code: Joi.number().required(),
            designation: Joi.string().required(),
            description: Joi.string().optional(),
            symptoms: Joi.string().required(),
          })
        ).optional(),
      }),
    }),
    (req, res, next) => {
      const token = req.headers['token'] as string;
      ctrl.saveClinicalDetails(token, req, res, next);
    }
  );

  route.get(
    '/filter',
    (req, res, next) => {
      const allergyID = req.headers['allergyid'] as string;
      const medicalConditionID = req.headers['medicalconditionid'] as string;
      const token = req.headers['token'] as string;
      ctrl.filterClinicalDetails(token, allergyID, medicalConditionID, res, next);
    }
	);

	// Send e-mail with password protected zip archive,
	// containing pdf with patient medical details
	// This one is purely for presentation purposes
	// as the clinical details are sent by the frontend
	route.get(
		'/sendClinicalDetailsPresentation',
		/* celebrate({
			body: Joi.object({
				patientMRN: Joi.string().required(),
				allergies: Joi.array().items(
					Joi.object({
						id: Joi.string().required(),
						name: Joi.string().required(),
						description: Joi.string().optional(),
					})
				).optional(),
				medicalConditions: Joi.array().items(
					Joi.object({
						code: Joi.number().required(),
						designation: Joi.string().required(),
						description: Joi.string().optional(),
						symptoms: Joi.string().required(),
					})
				).optional(),
			}),
		}), */
		(req, res, next) => {
			const token = req.headers['token'] as string;
			const password = req.headers['password'] as string;
			console.log("reached route");
			ctrl.sendClinicalDetailsPresentation(token, password, res, next);
		}
	);

	// Send e-mail with password protected zip archive,
	// containing pdf with patient medical details
	route.get(
		'/sendClinicalDetails',
		(req, res, next) => {
			const token = req.headers['token'] as string;
			const code = req.headers['code'] as string;
			const password = req.headers['password'] as string;
			ctrl.sendClinicalDetails(token, code, password, res, next);
		}
	);

  // Get clinical details by code
  route.get(
    '/:code',
    (req, res, next) => {
      const token = req.headers['token'] as string;
      const { code } = req.params;
	  ctrl.getClinicalDetailsByMRN(token, code, res, next);
    }
  );
};
