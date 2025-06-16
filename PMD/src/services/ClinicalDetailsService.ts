/* eslint-disable prettier/prettier */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prettier/prettier */
import { Service, Inject } from 'typedi';
import { ClinicalDetails } from '../domain/ClinicalDetails';
import IClinicalDetailsService from './IServices/IClinicalDetailsService';
import IClinicalDetailsRepo from './IRepos/IClinicalDetailsRepo';
import { IClinicalDetailsDTO } from '../dto/IClinicalDetailsDTO';
import config from '../../config';
import { ClinicalDetailsMap } from '../mappers/ClinicalDetailsMap';
import { filter } from 'lodash';
import { promises as fsPromises } from 'fs';
import * as fs from 'fs';
import { PDFDocument, StandardFonts } from 'pdf-lib';
import * as archiver from 'archiver';
import archiverZipEncrypted from 'archiver-zip-encrypted';
declare module 'archiver' {
	interface CoreOptions {
		encryptionMethod?: 'aes256' | 'zip20' | undefined;
		password?: string | undefined;
	}
}
archiver.registerFormat('zip-encrypted', archiverZipEncrypted);

@Service()
class ClinicalDetailsService implements IClinicalDetailsService {
	constructor(
		@Inject(config.repos.clinicalDetails.name) private clinicalDetailsRepo: IClinicalDetailsRepo
	) { }

	public async save(clinicalDetailsDto: any): Promise<IClinicalDetailsDTO> {
		const clinicalDetails = await ClinicalDetailsMap.toDomain(clinicalDetailsDto);
		console.log(clinicalDetails);
		console.log('============ Save - Service ============');
		try {
			console.log('Patient MRN: ' + clinicalDetails.id);
			const clinicalDetailsOrError = ClinicalDetails.createWithDetails(clinicalDetails.allergies, clinicalDetails.medicalConditions, clinicalDetails.id.toString());
			if (clinicalDetailsOrError.isFailure) {
				throw new Error(clinicalDetailsOrError.errorValue().toString());
			}
			const result = await this.clinicalDetailsRepo.save(clinicalDetailsOrError.getValue());
			return ClinicalDetailsMap.toDTO(result);
		} catch (e) {
			throw new Error(`Error saving ClinicalDetails [Service]: ${e.message}`);
		}
	}

	public async createBlank(mrn: string): Promise<IClinicalDetailsDTO> {
		const clinicalDetailsOrError = ClinicalDetails.createBlank(mrn);
		try {
			if (clinicalDetailsOrError.isFailure) {
				throw new Error(clinicalDetailsOrError.errorValue().toString());
			}
			const result = await this.clinicalDetailsRepo.save(clinicalDetailsOrError.getValue());
			return ClinicalDetailsMap.toDTO(result);
		} catch (e) {
			throw new Error(`Error saving ClinicalDetails [Service]: ${e.message}`);
		}
	}

	public async findByDomainId(code: string): Promise<IClinicalDetailsDTO | null> {
		try {
			const result = await this.clinicalDetailsRepo.findByDomainId(code);
			return ClinicalDetailsMap.toDTO(result);
		} catch (e) {
			throw new Error(`Error finding ClinicalDetails by Code [Service]: ${e.message}`);
		}
	}

	public async filterByValues(allergyID: string, conditionID: string): Promise<IClinicalDetailsDTO[] | null> {
		try {
			const result = await this.clinicalDetailsRepo.getAll();
			const detailsDTOs = result.map(cd => ClinicalDetailsMap.toDTO(cd));

			const filteredDTOs: IClinicalDetailsDTO[] = [];

			detailsDTOs.forEach(element => {
				let isValid = false;
				let stop = false;
				if (allergyID) {
					element.allergies.forEach(allergy => {
						console.log(allergy.id);
						if (allergy.id === allergyID) {
							isValid = true;
							stop = false;
						}
						else if (!isValid) stop = true;
					});
				}

				if (conditionID) {
					isValid = false;
					element.medicalConditions.forEach(condition => {
						if (condition.code.toString() === conditionID) {
							isValid = true;
							stop = false;
						}
						else if (!isValid) stop = true;
					});
				}

				if ((isValid && !stop) || (!allergyID && !conditionID)) {
					filteredDTOs.push(element);
				}

			});

			return filteredDTOs;

		} catch (e) {
			throw new Error(`Error finding ClinicalDetails by Code [Service]: ${e.message}`);
		}
	}

	public async sendClinicalDetails(code: string, password: string): Promise<IClinicalDetailsDTO | null> {
		console.log("Received send request");
		const outputDir = "./exports/"
		try {
			const clcDet = await this.clinicalDetailsRepo.findByDomainId(code);
			if (clcDet == null)
				return;

			const patientPdf = await PDFDocument.create();
			const pdfFont = await patientPdf.embedFont(StandardFonts.Courier);
			// 595 x 842 
			const pageWidth = 595;
			const pageHeight = 842;
			const vertSpace = 30;

			let pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
			// pdfPage.setFont(pdfFont);
			let yPos = pageHeight - 50;

			pdfPage.drawText('Patient Medical Record', {
				x: 50,
				y: yPos,
				size: 22,
				font: pdfFont
			});
			yPos -= vertSpace;

			pdfPage.drawText(`Patient Medical Number: ${clcDet.id}`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			pdfPage.drawText(`Medical Conditions:`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			if (clcDet.medicalConditions.length == 0) {
				pdfPage.drawText(`No medical conditions recorded for this patient`, {
					x: 50,
					y: yPos,
					size: 14,
					font: pdfFont
				});
			}
			else {
				const spacing = 20;
				clcDet.medicalConditions.forEach(mc => {
					pdfPage.drawText(`- ${mc.designation}`, {
						x: 50,
						y: yPos,
						size: 12,
						font: pdfFont
					});
					yPos -= spacing;
					if (yPos < 50) {
						yPos = pageHeight - 50;
						pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
					};
				});
			}
			yPos -= vertSpace;

			pdfPage.drawText(`Allergies:`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			if (clcDet.allergies.length == 0) {
				pdfPage.drawText(`No allergies recorded for this patient`, {
					x: 50,
					y: yPos,
					size: 14,
					font: pdfFont
				});
			}
			else {
				const spacing = 20;
				clcDet.allergies.forEach(al => {
					pdfPage.drawText(`- ${al.name}`, {
						x: 50,
						y: yPos,
						size: 12,
						font: pdfFont
					});
					yPos -= spacing;
					if (yPos < 50) {
						yPos = pageHeight - 50;
						pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
					};
				});
			}
			yPos -= vertSpace;

			const pdfBytes = await patientPdf.save();

			const dateTime = new Date();

			const pdfName = `${clcDet.id}_${dateTime.getFullYear()}_${dateTime.getMonth() + 1}_${dateTime.getDate().toString()}_info.pdf`;
			const zipName = `${clcDet.id}_${dateTime.getFullYear()}_${dateTime.getMonth() + 1}_${dateTime.getDate().toString()}_info_encrypted.zip`;

			await fsPromises.writeFile(`${outputDir}${pdfName}`, pdfBytes);

			// Step 2: Create a writable stream for the encrypted ZIP file
			const output = fs.createWriteStream(`${outputDir}${zipName}`);
			const archive = archiver.create('zip-encrypted', {
				zlib: { level: 8 }, // Compression level
				encryptionMethod: 'aes256', // Encryption method
				password: password, // Password for encryption
			});

			// Handle errors
			archive.on('error', (err) => {
				throw err;
			});

			// Pipe the archive to the output stream (ZIP file)
			archive.pipe(output);

			// Step 3: Append the PDF to the ZIP archive
			archive.append(await fsPromises.readFile(`${outputDir}${pdfName}`), { name: `${pdfName}` });

			// Finalize the archive (close the stream and finish the ZIP creation)
			await archive.finalize();

			await new Promise((resolve, reject) => {
				output.on('close', resolve);
				archive.on('error', reject);
			});
			return ClinicalDetailsMap.toDTO(clcDet);
		} catch (e) {
			throw new Error(`Error finding ClinicalDetails by Code [Service]: ${e.message}`);
		}
	}

	public async sendClinicalDetailsDemonstration(password: string): Promise<string | null> {
		console.log("Received send request (presentation)");
		const clcDet: IClinicalDetailsDTO = {
			patientMRN: "MRN123456789",
			allergies: [{
				id: "1",
				name: "pollen",
				description: "common allergy to pollen"
			}, {
				id: "2",
				name: "dust",
				description: "common allergy to dust"
			}],
			medicalConditions: [{
				code: 123456,
				designation: "Myopia",
				description: "short-sightedness",
				symptoms: ""
			}, {
				code: 654321,
				designation: "Hypermetropia",
				description: "farsightedness",
				symptoms: ""
			}]
		};

		const outputDir = "./exports/"

		try {
			const patientPdf = await PDFDocument.create();
			const pdfFont = await patientPdf.embedFont(StandardFonts.Courier);
			// 595 x 842 
			const pageWidth = 595;
			const pageHeight = 842;
			const vertSpace = 30;

			let pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
			// pdfPage.setFont(pdfFont);
			let yPos = pageHeight - 50;

			pdfPage.drawText('Patient Medical Record', {
				x: 50,
				y: yPos,
				size: 22,
				font: pdfFont
			});
			yPos -= vertSpace;

			pdfPage.drawText(`Patient Medical Number: ${clcDet.patientMRN}`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			pdfPage.drawText(`Medical Conditions:`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			if (clcDet.medicalConditions.length == 0) {
				pdfPage.drawText(`No medical conditions recorded for this patient`, {
					x: 50,
					y: yPos,
					size: 14,
					font: pdfFont
				});
			}
			else {
				const spacing = 20;
				clcDet.medicalConditions.forEach(mc => {
					pdfPage.drawText(`- ${mc.designation}`, {
						x: 50,
						y: yPos,
						size: 12,
						font: pdfFont
					});
					yPos -= spacing;
					if (yPos < 50) {
						yPos = pageHeight - 50;
						pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
					};
				});
			}
			yPos -= vertSpace;

			pdfPage.drawText(`Allergies:`, {
				x: 50,
				y: yPos,
				size: 14,
				font: pdfFont
			});
			yPos -= vertSpace;

			if (clcDet.allergies.length == 0) {
				pdfPage.drawText(`No allergies recorded for this patient`, {
					x: 50,
					y: yPos,
					size: 14,
					font: pdfFont
				});
			}
			else {
				const spacing = 20;
				clcDet.allergies.forEach(al => {
					pdfPage.drawText(`- ${al.name}`, {
						x: 50,
						y: yPos,
						size: 12,
						font: pdfFont
					});
					yPos -= spacing;
					if (yPos < 50) {
						yPos = pageHeight - 50;
						pdfPage = patientPdf.addPage([pageWidth, pageHeight]);
					};
				});
			}
			yPos -= vertSpace;

			const pdfBytes = await patientPdf.save();

			const dateTime = new Date();

			const pdfName = `${clcDet.patientMRN}_${dateTime.getFullYear()}_${dateTime.getMonth() + 1}_${dateTime.getDate().toString()}_info.pdf`;
			const zipName = `${clcDet.patientMRN}_${dateTime.getFullYear()}_${dateTime.getMonth() + 1}_${dateTime.getDate().toString()}_info_encrypted.zip`;

			await fsPromises.writeFile(`${outputDir}${pdfName}`, pdfBytes);

			// Step 2: Create a writable stream for the encrypted ZIP file
			const output = fs.createWriteStream(`${outputDir}${zipName}`);
			const archive = archiver.create('zip-encrypted', {
				zlib: { level: 8 }, // Compression level
				encryptionMethod: 'aes256', // Encryption method
				password: password, // Password for encryption
			});

			// Handle errors
			archive.on('error', (err) => {
				throw err;
			});

			// Pipe the archive to the output stream (ZIP file)
			archive.pipe(output);

			// Step 3: Append the PDF to the ZIP archive
			archive.append(await fsPromises.readFile(`${outputDir}${pdfName}`), { name: `${pdfName}` });

			// Finalize the archive (close the stream and finish the ZIP creation)
			await archive.finalize();

			await new Promise((resolve, reject) => {
				output.on('close', resolve);
				archive.on('error', reject);
			});


			return zipName;
		} catch (e) {
			throw new Error(`Error in sendClinicalDetailsDemonstration`);
		}
	}
}

export default ClinicalDetailsService;