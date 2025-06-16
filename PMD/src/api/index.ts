/* eslint-disable prettier/prettier */
import { Router } from 'express';
import auth from './routes/userRoute';
import user from './routes/userRoute';
import role from './routes/roleRoute';
import allergy from './routes/allergyRoute';
import condition from './routes/conditionRoute';
import clinicalDetails from './routes/clinicalDetailsRoute';

export default () => {
	const app = Router();

	auth(app);
	user(app);
	role(app);
	allergy(app);
	condition(app);
	clinicalDetails(app)
	
	return app
}