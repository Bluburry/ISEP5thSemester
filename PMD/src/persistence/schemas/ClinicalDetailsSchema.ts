/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */

import mongoose from 'mongoose';
import { IClinicalDetailsPersistence } from '../../dataschema/IClinicalDetailsPersistence';

const ClinicalDetailsSchema = new mongoose.Schema(
  {
    patientMRN: { type: String, unique: true, required: true }, // Unique ID for the clinical details
    allergies: {
      type: [String], // Array of strings
      default: null, // Default to null if not provided
    },
    medicalConditions: {
      type: [String], // Array of strings
      default: null, // Default to null if not provided
    },
  },
  {
    timestamps: true, // Automatically adds `createdAt` and `updatedAt` fields
  },
);

export default mongoose.model<IClinicalDetailsPersistence & mongoose.Document>('ClinicalDetails', ClinicalDetailsSchema);