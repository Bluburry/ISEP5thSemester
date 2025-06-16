import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AdminService } from '../../admin.service';
import { OperationRoomTypeData } from '../../interfaces/room-type-data';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-operation-room-type-create',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './operation-room-type-create.component.html',
  styleUrls: ['./operation-room-type-create.component.css']
})
export class OperationRoomTypeCreateComponent implements OnInit {
  token: string = '';
  correct: boolean = false;
  wrong: boolean = false;
  errorMessage: string | null = null;

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.token = localStorage.getItem('authToken') || this.token;
  }

  createOperationRoomType(form: NgForm) {
    const formValue = form.value;
    const operationRoomTypeData: OperationRoomTypeData = {
      OpCode: formValue.code,
      name: formValue.name,
      description: formValue.description
    };

    this.adminService.createOperationRoomType(this.token, operationRoomTypeData).subscribe({
      next: () => {
        this.correct = true;
        this.wrong = false;
        this.errorMessage = null;
        console.log('Operation room type created');
        form.reset();
      },
      error: (error) => {
        this.correct = false;
        this.wrong = true;
        this.errorMessage = error.error.message || 'Error creating room type. Please try again.';
        console.log('Error:', this.errorMessage);
      }
    });
  }
}