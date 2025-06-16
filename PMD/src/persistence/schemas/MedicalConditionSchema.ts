/* eslint-disable prettier/prettier */

import mongoose from 'mongoose';
import { IMedicalConditionPersistence } from '../../dataschema/IMedicalConditionPersistence';

const MedicalConditionSchema = new mongoose.Schema(
  {
    code: { type: Number, unique: true, required: true }, // Unique ID for the Condition
    designation: { type: String, required: true }, // The Medical Condition name (required)
    description: { type: String, required: false, default: null }, // Description (can be optional, default is null)
    symptoms: {type: String, required: true, default: null} // symptoms (shouldn't be optional, so it isn't)
  },
  {
    timestamps: true, // Automatically adds `createdAt` and `updatedAt` fields
  },
);

export default mongoose.model<IMedicalConditionPersistence & mongoose.Document>('MedicalCondition', MedicalConditionSchema);
