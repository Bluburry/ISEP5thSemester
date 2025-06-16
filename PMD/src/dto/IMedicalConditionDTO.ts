/* eslint-disable prettier/prettier */
export default interface IMedicalConditionDTO {
  code: number;
  designation: string;
  description?: string | null;
  symptoms: string;
}
