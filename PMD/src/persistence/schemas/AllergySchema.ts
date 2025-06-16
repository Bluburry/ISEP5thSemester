/* eslint-disable prettier/prettier */

import mongoose from 'mongoose';
import { IAllergyPersistence } from '../../dataschema/IAllergyPestistence';

const AllergySchema = new mongoose.Schema(
  {
    domainId: { type: String, unique: true, required: true }, // Unique ID for the allergy
    name: { type: String, required: true }, // Allergy name (required)
    description: { type: String, required: false, default: null }, // Description (optional, default is null)
  },
  {
    timestamps: true, // Automatically adds `createdAt` and `updatedAt` fields
  },
);

export default mongoose.model<IAllergyPersistence & mongoose.Document>('Allergy', AllergySchema);
