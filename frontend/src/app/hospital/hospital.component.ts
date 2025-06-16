// import { Component, provideZoneChangeDetection } from '@angular/core';
import { AfterViewInit, Component, ElementRef, HostListener, OnDestroy, ViewChild } from
	'@angular/core';
import * as THREE from "three";
import * as TWEEN from '@tweenjs/tween.js';
import layoutData from '../../../layout/hospitalMap1.json'
import { GLTFLoader } from 'three-stdlib';
import { OrbitControls } from 'three-stdlib';
import { HospitalService } from './hospital.service';
import { AvailabilitySlots } from './hospital-data';
import { AppointmentDto } from '../Admin/interfaces/operation-type-data';
import { CommonModule } from '@angular/common';
import { OperationRequestData } from '../Doctor/interfaces/operation-request-data';
import { DoctorService } from '../Doctor/doctor.service';

@Component({
	selector: 'app-hospital',
	standalone: true,
	templateUrl: './hospital.component.html',
	styleUrls: ['./hospital.component.scss'],
	imports: [CommonModule], // Add CommonModule here
})

export class HospitalComponent implements AfterViewInit, OnDestroy {
	constructor(
		private hospitalService: HospitalService,
		private doctorService: DoctorService
	) { }

	@ViewChild('myCanvas') private canvasRef!: ElementRef<HTMLCanvasElement>;

	private get canvas(): HTMLCanvasElement {
		return this.canvasRef.nativeElement;
	}

	private renderer!: THREE.WebGLRenderer;
	private camera!: THREE.PerspectiveCamera;
	private controls!: OrbitControls;
	private scene!: THREE.Scene;
	private ground!: THREE.Mesh;

	private frustumSize = 20;

	private wall = new THREE.Group();
	private door = new THREE.Group();
	private layout = new THREE.Group();
	private labels = new THREE.Group();
	private scale = new THREE.Vector3(1.0, 1.0, 1.0);
	private bedScale = new THREE.Vector3(0.02, 0.02, 0.02);
	private patientScale = new THREE.Vector3(0.0062, 0.0062, 0.0062);
	private nurseScale = new THREE.Vector3(0.006, 0.006, 0.006);
	private distanceFromTable = 0.50;

	private raycaster = new THREE.Raycaster();
	private normalPos = new THREE.Vector2();
	private clickableObjs: THREE.Object3D[] = [];
	/* private pickedObject = new THREE.Object3D();
	private pickedObjectSavedColor = 0; */

	private camera_x = 20;
	private camera_y = 30;
	private camera_z = 30;

	operationRooms: AvailabilitySlots[] = [];
	selectedAppointment: AppointmentDto | null = null;
	selectedRequest: OperationRequestData | null = null;
	private getAspectRatio(): number {
		return this.canvas.clientWidth / this.canvas.clientHeight;
	}

	private createScene(): void {
		this.scene = new THREE.Scene();
		this.scene.background = new THREE.Color(0xa0a0a0);

		this.renderer = new THREE.WebGLRenderer({ canvas: this.canvas });
		this.renderer.setSize(this.canvas.clientWidth, this.canvas.clientHeight);

		this.camera = new THREE.PerspectiveCamera(45, this.canvas.clientWidth / this.canvas.clientHeight, 1, 10000);

		this.controls = new OrbitControls(this.camera, this.renderer.domElement);

		this.controls.mouseButtons = {
			MIDDLE: THREE.MOUSE.DOLLY,
			RIGHT: THREE.MOUSE.ROTATE
		}

		this.controls.update();

		// Position the camera above the.layout, looking down
		this.camera.position.set(this.camera_x, this.camera_y, this.camera_z); // Higher Y position to hover over.layout
		this.camera.lookAt(0, 0, 0); // Look down at the center of the scene

		// Initialize the renderer

		this.renderer.setPixelRatio(devicePixelRatio);
		this.renderer.shadowMap.enabled = true;

		const ambientLight = new THREE.AmbientLight(0xFFFFFF, 0.1);
		this.scene.add(ambientLight);

		const directionalLight = new THREE.DirectionalLight(0xFFFFFF, 2.5); // Bright light
		directionalLight.position.set(-20, 30, 40); // Elevated position for better shadow angles
		directionalLight.castShadow = true;

		// Shadow map settings
		directionalLight.shadow.mapSize.width = 8192; // High resolution for sharper shadows
		directionalLight.shadow.mapSize.height = 8192;
		directionalLight.shadow.camera.near = 0.5;
		directionalLight.shadow.camera.far = 200;


		// Adjust shadow camera bounds for directional light
		directionalLight.shadow.camera.left = -20;
		directionalLight.shadow.camera.right = 20;
		directionalLight.shadow.camera.top = 10;
		directionalLight.shadow.camera.bottom = -10;

		// Add the light to the scene
		this.scene.add(directionalLight);
	}

	private createWall(textureUrl: any): void {
		// Create a texture
		const texture = new THREE.TextureLoader().load(textureUrl);
		texture.colorSpace = THREE.SRGBColorSpace;
		texture.magFilter = THREE.LinearFilter;
		texture.minFilter = THREE.LinearMipMapLinearFilter;

		let height = 1.0, width = 0.5, depth = 0.025;

		let geometry1 = new THREE.PlaneGeometry(width, height);

		let material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: texture });
		let face1 = new THREE.Mesh(geometry1, material);
		face1.position.set(0.0, 0.0, depth);

		this.wall.add(face1);

		let face2 = new THREE.Mesh().copy(face1, false);
		face2.rotation.y = Math.PI;
		face2.position.set(0, 0, -depth);
		this.wall.add(face2);

		let points = new Float32Array([
			-width / 2, -height / 2, depth,
			-width / 2, height / 2, depth,
			-width / 2, height / 2, -depth,
			-width / 2, -height / 2, -depth,
		]);
		let normals = new Float32Array([
			-1, 0, 0,  // Normal for vertex 0
			-1, 0, 0,  // Normal for vertex 1
			-1, 0, 0,  // Normal for vertex 2
			-1, 0, 0   // Normal for vertex 3
		]);
		let indices = [
			0, 1, 2,
			2, 3, 0,
		];

		let geometry2 = new THREE.BufferGeometry().setAttribute("position", new THREE.BufferAttribute(points, 3));
		geometry2.setAttribute("normal", new THREE.BufferAttribute(normals, 3));
		geometry2.setIndex(indices);

		material = new THREE.MeshPhongMaterial({ color: 0x000000 });
		let face3 = new THREE.Mesh(geometry2, material);

		this.wall.add(face3);

		let face4 = new THREE.Mesh().copy(face3, false);
		face4.rotation.y = Math.PI;
		this.wall.add(face4);

		points = new Float32Array([
			-width / 2, height / 2, -depth,
			-width / 2, height / 2, depth,
			width / 2, height / 2, -depth,
			width / 2, height / 2, depth,
		]);
		normals = new Float32Array([
			0, 1, 0,
			0, 1, 0,
			0, 1, 0,
			0, 1, 0
		]);
		indices = [
			0, 1, 3,
			0, 3, 2,
		];

		let geometry3 = new THREE.BufferGeometry().setAttribute("position", new THREE.BufferAttribute(points, 3));
		geometry3.setAttribute("normal", new THREE.BufferAttribute(normals, 3));
		geometry3.setIndex(indices);
		let face5 = new THREE.Mesh(geometry3, material);

		face1.castShadow = true;
		face2.castShadow = true;
		face3.castShadow = true;
		face4.castShadow = true;
		face5.castShadow = true;
		this.wall.castShadow

		this.wall.add(face5);
	}

	private createDoor(frontTextureUrl: any, backTextureUrl: any): void {
		let height = 0.8, width = 0.6, depth = 0.07;

		let geometry1 = new THREE.PlaneGeometry(width, height);

		const frontTexture = new THREE.TextureLoader().load(frontTextureUrl);
		frontTexture.colorSpace = THREE.SRGBColorSpace;
		frontTexture.magFilter = THREE.LinearFilter;
		frontTexture.minFilter = THREE.LinearMipMapLinearFilter;

		const backTexture = new THREE.TextureLoader().load(backTextureUrl);
		backTexture.colorSpace = THREE.SRGBColorSpace;
		backTexture.magFilter = THREE.LinearFilter;
		backTexture.minFilter = THREE.LinearMipMapLinearFilter;

		let material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: frontTexture });
		let face1 = new THREE.Mesh(geometry1, material);
		face1.position.set(0.0, 0.0, depth);

		this.door.add(face1);

		material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: backTexture });
		let face2 = new THREE.Mesh(geometry1, material);
		face2.rotation.y = Math.PI;
		face2.position.set(0, 0, -depth);
		this.door.add(face2);

		let points = new Float32Array([
			-width / 2, -height / 2, depth,
			-width / 2, height / 2, depth,
			-width / 2, height / 2, -depth,
			-width / 2, -height / 2, -depth,
		]);
		let normals = new Float32Array([
			-1, 0, 0,  // Normal for vertex 0
			-1, 0, 0,  // Normal for vertex 1
			-1, 0, 0,  // Normal for vertex 2
			-1, 0, 0   // Normal for vertex 3
		]);
		let indices = [
			0, 1, 2,
			2, 3, 0,
		];

		let geometry2 = new THREE.BufferGeometry().setAttribute("position", new THREE.BufferAttribute(points, 3));
		geometry2.setAttribute("normal", new THREE.BufferAttribute(normals, 3));
		geometry2.setIndex(indices);

		material = new THREE.MeshPhongMaterial({ color: 0xb7c7c6 });
		let face3 = new THREE.Mesh(geometry2, material);

		this.door.add(face3);

		let face4 = new THREE.Mesh().copy(face3, false);
		face4.rotation.y = Math.PI;

		this.door.add(face4);

		points = new Float32Array([
			-width / 2, height / 2, -depth,
			-width / 2, height / 2, depth,
			width / 2, height / 2, -depth,
			width / 2, height / 2, depth,
		]);
		normals = new Float32Array([
			0, 1, 0,
			0, 1, 0,
			0, 1, 0,
			0, 1, 0
		]);
		indices = [
			0, 1, 3,
			0, 3, 2,
		];

		let geometry3 = new THREE.BufferGeometry().setAttribute("position", new THREE.BufferAttribute(points, 3));
		geometry3.setAttribute("normal", new THREE.BufferAttribute(normals, 3));
		geometry3.setIndex(indices);
		let face5 = new THREE.Mesh(geometry3, material);

		face1.castShadow = true;
		face2.castShadow = true;
		face3.castShadow = true;
		face4.castShadow = true;
		face5.castShadow = true;

		this.door.add(face5);
	}

	private createFloor(size: any, floorTexture: any): void {
		//Texture
		const texture = new THREE.TextureLoader().load(floorTexture);
		texture.colorSpace = THREE.SRGBColorSpace;
		texture.wrapS = THREE.RepeatWrapping;
		texture.wrapT = THREE.RepeatWrapping;
		texture.repeat.set(size.width, size.height);
		texture.magFilter = THREE.LinearFilter;
		texture.minFilter = THREE.LinearMipmapLinearFilter;

		const geometry = new THREE.PlaneGeometry(size.width, size.height);
		const material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: texture });

		this.ground = new THREE.Mesh(geometry, material);
		this.ground.rotation.x = -Math.PI / 2;
		this.ground.receiveShadow = true;
	}

	private makeLabelCanvas(text: string): HTMLCanvasElement {
		let borderSize = 5;
		let size = 36;
		let canvas = document.createElement('canvas')
		let ctx = canvas.getContext('2d');
		if (ctx == null)
			return canvas;

		let font = `${size}px bold sans-serif`;
		ctx.font = font;
		// measure how long the name will be
		let doubleBorderSize = borderSize * 2;
		let width = ctx.measureText(text).width + doubleBorderSize;
		let height = size + doubleBorderSize;
		ctx.canvas.width = width;
		ctx.canvas.height = height;

		// need to set font again after resizing canvas
		ctx.font = font;
		ctx.textBaseline = 'top';

		ctx.fillStyle = 'rgba(0, 100, 0, 0.5)';
		ctx.fillRect(0, 0, width, height);
		ctx.fillStyle = 'white';
		ctx.fillText(text, borderSize, borderSize);

		return ctx.canvas;
	}

	private makeLabels(text: string): THREE.Sprite {
		let canvas = this.makeLabelCanvas(text);
		let texture = new THREE.CanvasTexture(canvas);
		// because our canvas is likely not a power of 2
		// in both dimensions set the filtering appropriately.
		texture.minFilter = THREE.LinearFilter;
		texture.wrapS = THREE.ClampToEdgeWrapping;
		texture.wrapT = THREE.ClampToEdgeWrapping;

		let labelMaterial = new THREE.SpriteMaterial({
			map: texture,
			transparent: true,
		});

		const label = new THREE.Sprite(labelMaterial);

		// if units are meters then 0.01 here makes size
		// of the label into centimeters.
		const labelBaseScale = 0.01;
		label.scale.x = canvas.width * labelBaseScale;
		label.scale.y = canvas.height * labelBaseScale;

		return label;
	}

	private loadLabels(rooms: any, width: number, height: number): void {
		rooms.forEach((room: { id: string; table: number[][] }) => {
			//console.log(room);
			//console.log(room.id);
			room.table.forEach((table: number[]) => {
				// console.log(table);
				let label = this.makeLabels(room.id);
				// label.position.set(table[1], 2, table[0]);
				label.position.set(table[1] - width / 2, 2, table[0] - height / 2);
				this.labels.add(label);
			});
		});
	}

	private createModel(modelUrl: string, scale: any): Promise<THREE.Group> {
		let model = new THREE.Group(); // Store the loaded model
		const loader = new GLTFLoader();
		return new Promise((resolve, reject) => {
			loader.load(
				modelUrl,
				(gltf) => {
					model = gltf.scene; // Store the loaded model
					model.scale.copy(scale); // Apply scaling
					model.traverse((child) => {
						if (child instanceof THREE.Mesh) {
							child.castShadow = true; // Enable casting shadows
							child.receiveShadow = true; // Enable receiving shadows
						}
					});
					resolve(model);
				},
				(xhr) => {
					console.log((xhr.loaded / xhr.total) * 100 + '% loaded');
				},
				(error) => {
					console.error('An error occurred while loading the model:', error);
					reject(error); // Reject the promise if an error occurs
				}
			);
		});
	}

	private async placeObjects(data: any): Promise<void> {
		let bedModel, patientModel/*, nurseModel*/;

		// Wait until the bed model is loaded
		try {
			[bedModel, patientModel, /*nurseModel*/] = await Promise.all([
				this.createModel(data.bedModel, this.bedScale),
				this.createModel(data.patientModel, this.patientScale),
				//this.createModel(data.nurseModel, this.nurseScale),
			]);
		} catch (error) {
			console.error('Error loading models:', error);
			return; // Exit if either model couldn't be loaded
		}

		// Build Objects
		let createdObject;
		data.rooms.forEach((room: { id: string; table: number[][] }) => {
			room.table.forEach(table => {
				createdObject = bedModel.clone();

				createdObject.traverse((child) => {
					if (child instanceof THREE.Mesh) {
						child.castShadow = true; // Enable casting shadows
						child.receiveShadow = true; // Enable receiving shadows
					}
				});

				createdObject.position.set(table[1] - (data.size.width + 0.5) / 2, 0.25, table[0] - (data.size.height - 3) / 2 - 1.25);
				if (table[2] == 2) {
					if (table[1] - (data.size.width + 0.5) / 2 > 0) {
						createdObject.rotation.y -= Math.PI;;
					}
				} else {
					if (table[0] - (data.size.height - 3) / 2 > 0) {
						createdObject.rotation.y += Math.PI / 2
					} else {
						createdObject.rotation.y -= Math.PI / 2
					}
				}

				// custom data to use with raycasting
				createdObject.userData = {
					roomID: data.layout[table[0]][table[1]],
					map_x: table[0],
					map_y: table[1]
				};
				this.layout.add(createdObject);
				this.clickableObjs.push(createdObject);
			});
		});

		const currentTimeInMinutes = new Date().getHours() * 60 + new Date().getMinutes();

		// Subscribe to the Observable and process the data
		this.hospitalService.getAllRoomAvailabilitySlots().subscribe({
			next: (list: AvailabilitySlots[]) => {
				list.forEach(element => {
					var exists: Boolean = false;

					this.operationRooms.forEach(rooms => {
						if(rooms.OperationRoomID === element.OperationRoomID) exists = true;
					});

					if(!exists) this.operationRooms.push(element);
						
				});
				console.log(this.operationRooms);
				data.rooms.forEach((room: { id: string; table: number[][] }) => {
					// Find the matching room in the list
					const matchingRoom = list.find((item: AvailabilitySlots) => {
						// Parse the value string into an array [startMinutes, endMinutes]
						const timeRange = JSON.parse(item.Value.replace(/([0-9]+)/g, '"$1"').replace("(", "[").replace(")", "]"));
						const startMinutes = timeRange[0];
						const endMinutes = timeRange[1];

						// Check if the roomName matches and if the current time is in the range
						return item.OperationRoomName === room.id &&
							currentTimeInMinutes >= startMinutes &&
							currentTimeInMinutes <= endMinutes;
					});

					if (matchingRoom) {
						// Trigger the logic for the matching room
						room.table.forEach(table => {
							const createdObject = patientModel.clone();
							createdObject.position.set(
								table[1] - (data.size.width + 0.5) / 2,
								0.325,
								table[0] - (data.size.height - 3) / 2 - 1.25
							);

							if (table[2] == 2) {
								if (table[1] - (data.size.width + 0.5) / 2 > 0) {
									createdObject.rotation.y -= Math.PI / 2;
								} else {
									createdObject.rotation.y += Math.PI / 2;
								}
							} else {
								if (table[0] - (data.size.height - 3) / 2 > 0) {
									createdObject.rotation.y -= Math.PI;
								}
							}

							this.layout.add(createdObject);
						});
					}
				});
			},
			error: (err) => {
				console.error('Error fetching room availability slots:', err);
			}
		});
	}

	private loadLayout(data: any): void {
		this.createFloor(data.size, data.groundTextureUrl);
		this.createWall(data.wallTextureUrl);
		this.createDoor(data.doorFrontTextureUrl, data.doorBackTextureUrl);

		// Build the maze
		let wallObject;

		for (let i = 0; i <= data.size.width; i++) {
			for (let j = 0; j <= data.size.height; j++) {
				// console.log("i", j, "j", i, data.layout[j][i]);
				// draw walls
				if (data.layout[j][i] == 1 || data.layout[j][i] == 2) {
					// wallObject.position.set(i - (data.size.width) / 2, 0.5, j - (data.size.height) / 2);
					// draw wall up
					if (j > 0 && (data.layout[j - 1][i] == 1 || data.layout[j - 1][i] == 2)) {
						// console.log("draw wall up");
						wallObject = this.wall.clone();
						wallObject.position.set(i - (data.size.width) / 2, 0.5, j - (data.size.height - 1) / 2 - 1.25);
						wallObject.rotation.y += Math.PI / 2;
						this.layout.add(wallObject);
					}
					// draw wall right
					if (i < data.size.width && (data.layout[j][i + 1] == 1 || data.layout[j][i + 1] == 2)) {
						// console.log("draw wall right");
						wallObject = this.wall.clone();
						wallObject.position.set(i - (data.size.width) / 2 + 0.75, 0.5, j - (data.size.height) / 2);
						this.layout.add(wallObject);
					}
					// draw wall down
					if (j < data.size.height && (data.layout[j + 1][i] == 1 || data.layout[j + 1][i] == 2)) {
						// console.log("draw wall down");
						wallObject = this.wall.clone();
						wallObject.position.set(i - (data.size.width) / 2, 0.5, j - (data.size.height - 1) / 2 + 0.25);
						wallObject.rotation.y += Math.PI / 2;
						this.layout.add(wallObject);
					}
					// draw wall left
					if (i > 0 && (data.layout[j][i - 1] == 1 || data.layout[j][i - 1] == 2)) {
						// console.log("draw wall left");
						wallObject = this.wall.clone();
						wallObject.position.set(i - (data.size.width) / 2 - 0.75, 0.5, j - (data.size.height) / 2);
						this.layout.add(wallObject);
					}
				}
				if (data.layout[j][i] == 2) {
					// horizontal
					if (j > 0 && j < data.size.height &&
						(data.layout[j - 1][i] == 1 || data.layout[j + 1][i] == 1 ||
							data.layout[j - 1][i] == 2 || data.layout[j + 1][i] == 2)) {
						if (j > 0 && data.layout[j - 1][i] != 2) {
							if (j < data.size.height && data.layout[j + 1][i] == 2) {
								wallObject = this.door.clone();
								wallObject.position.set(i - (data.size.width) / 2, 0.4, j + 0.6 - (data.size.height) / 2);
								wallObject.rotation.y += Math.PI * 3 / 2;
								this.layout.add(wallObject);
							}
							wallObject = this.door.clone();
							wallObject.position.set(i - (data.size.width) / 2, 0.4, j - (data.size.height) / 2);
							wallObject.rotation.y += Math.PI / 2;
							this.layout.add(wallObject);
						}
					}
					// vertical
					else {
						if (i > 0 && data.layout[j][i - 1] != 2) {
							wallObject = this.door.clone();
							wallObject.position.set(i - (data.size.width) / 2, 0.4, j - (data.size.height) / 2);
							if (i < data.size.width && data.layout[j][i + 1] == 2) {
								wallObject.rotation.y += Math.PI;
								this.layout.add(wallObject);
								wallObject = this.door.clone();
								wallObject.position.set(i + 0.6 - (data.size.width) / 2, 0.4, j - (data.size.height) / 2);
								this.layout.add(wallObject);
							}
							else
								this.layout.add(wallObject);
						}
					}
				}
			}
		}

		this.loadLabels(data.rooms, data.size.width, data.size.height);


		this.layout.scale.set(this.scale.x, this.scale.y, this.scale.z);
	}

	animate() {
		//requestAnimationFrame(this.animate.bind(this));
		requestAnimationFrame(() => this.render());
		TWEEN.update();
		this.controls.update();
		this.renderer.render(this.scene, this.camera);
	}

	private render(): void {
		this.scene.add(this.ground);
		this.scene.add(this.layout);
		this.scene.add(this.labels);
		this.animate();
	}

	async ngAfterViewInit(): Promise<void> {
		this.createScene();
		this.loadLayout(layoutData);
		await this.placeObjects(layoutData);
		this.render();
		// window.addEventListener('click', this.onMouseClick.bind(this));
		window.addEventListener('click', (event: MouseEvent) => {
			this.onMouseClick(event, layoutData);
		});

		const display = document.getElementById('display');
		window.addEventListener('keydown', (event) => {
			if (event.key.toLowerCase() === 'i' && display) {
			  if (display.style.display === 'none') {
				display.style.display = 'block';
			  } else {
				display.style.display = 'none';
			  }
			}
		});
		// window.addEventListener('resize', () => { this.resize(this.renderer, this.canvas); });
	}

	private onMouseClick(event: MouseEvent, data: any) {
		if (event.button !== 0) return;
	
		const rect = this.canvas.getBoundingClientRect();
		const helper = new THREE.Vector2(
			((event.clientX - rect.left) * this.canvas.width) / rect.width,
			((event.clientY - rect.top) * this.canvas.height) / rect.height
		);
	
		this.normalPos.set(
			(helper.x / this.canvas.width) * 2 - 1,
			(helper.y / this.canvas.height) * -2 + 1
		);
	
		this.raycaster.setFromCamera(this.normalPos, this.camera);
		const intersectedObjects = this.raycaster.intersectObjects(this.clickableObjs);
	
		if (intersectedObjects.length) {
			let test = intersectedObjects[0].object;
	
			while (test.parent) {
				if (test.userData && test.userData['roomID'] !== undefined) {
					break;
				}
				test = test.parent;
			}

			if (!test.parent)
				return;

			let bedInfo = test.userData;
			this.selectedAppointment = null;
			this.selectedRequest = null;
			this.operationRooms.forEach(element => {
				if(element.OperationRoomName === bedInfo['roomID'] && element.OperationRoomID) {
					this.hospitalService.getAppointmentByRoom(element.OperationRoomID).subscribe({
						next: (appointment: AppointmentDto | null) => {
							if(appointment){
								this.selectedAppointment = appointment;
								let token = localStorage.getItem('authToken');
								if(token){
									this.doctorService.getOperationById(this.selectedAppointment.OperationRequestId, token);
									this.doctorService.selectedRequest$.subscribe((opRequest: OperationRequestData | null) => {
										this.selectedRequest = opRequest;
									});
								}
							}
						}
					});
				}
			});
			console.log(this.selectedRequest)
			console.log(bedInfo['roomID']);
			// i - (data.size.width) / 2
			// j - (data.size.height - 1) / 2
			// let x = bedInfo['map_x'] - (data.size.width) / 2;
			// let y = bedInfo['map_y'] - (data.size.height) / 2;

			console.log(bedInfo, test.position);

			// this.camera.position.set(this.camera_x, this.camera_y, this.camera_z);
			// this.camera.position.set(test.position.x, test.position.y, test.position.z);

			
			// calculating room center position
			let x = bedInfo['map_x'];
			let y = bedInfo['map_y'];
			let x_start = x, x_end = x, y_start = y, y_end = y;
	
			while (data.layout[x_start][y] === bedInfo['roomID']) x_start--;
			while (data.layout[x_end][y] === bedInfo['roomID']) x_end++;
			while (data.layout[x][y_start] === bedInfo['roomID']) y_start--;
			while (data.layout[x][y_end] === bedInfo['roomID']) y_end++;
	
			const targetPos = new THREE.Vector3(
				(y_start + (y_end - y_start) / 2) - data.size.width / 2, // Center X
				0,                                                     	 // Ground level for lookAt
				(x_start + (x_end - x_start) / 2) - data.size.height / 2 // Center Z
			);
	
			const currentPos = this.camera.position.clone();
			const cameraHeight = 12;
			const currentTarget = this.controls.target.clone();
	
			// Animate camera position
			new TWEEN.Tween(currentPos)
			.to({ x: targetPos.x + 1, y: cameraHeight, z: targetPos.z + 1 }, 1000) // Final position with updated height
			.easing(TWEEN.Easing.Quadratic.InOut)
			.onUpdate(() => this.camera.position.set(currentPos.x, currentPos.y, currentPos.z))
			.start();

			// Animate controls target to face straight down at the room
			new TWEEN.Tween(currentTarget)
			.to({ x: targetPos.x, y: targetPos.y, z: targetPos.z }, 1000) // Look straight at the room center
			.easing(TWEEN.Easing.Quadratic.InOut)
			.onUpdate(() => this.controls.target.set(currentTarget.x, currentTarget.y, currentTarget.z))
			.onComplete(() => this.controls.update())
			.start();

			// this.controls.target.copy(test.position);
			this.controls.target.copy(targetPos);
		}
	}

	ngOnDestroy(): void {
		// window.removeEventListener('click', this.onMouseClick.bind(this));
		window.removeEventListener('click', (event: MouseEvent) => {
			this.onMouseClick(event, layoutData);
		});
		// window.removeEventListener('resize', () => { this.resize(this.renderer, this.canvas); });
		this.renderer.dispose();
	}

	/* @HostListener('mousedown', ['$event'])
	onMouseClick(): void {
		if (event.buttons = 1)
			console.log('mouse clicked');
		console.log(event);
	} */

	// New method to handle resizing
	/* @HostListener('window:resize', ['$event'])
	onResize(): void {
		this.resize();
	} */

	/* private resize(renderer: THREE.WebGLRenderer, canvas: HTMLCanvasElement): void {
		renderer.setSize(canvas.clientWidth, canvas.clientHeight, false);

		console.log('help');
		this.scene.add(this.ground);
		this.scene.add(this.layout);
		this.scene.add(this.labels);
		
		this.controls.update();
		this.camera.updateProjectionMatrix();

		// requestAnimationFrame(this.animate.bind(this));

		this.controls.update();
		this.renderer.render(this.scene, this.camera);

		// this.renderer.render(this.scene, this.camera);

	} */

}
