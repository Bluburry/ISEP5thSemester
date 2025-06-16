import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationRoomTypeCreateComponent } from './operation-room-type-create.component';

describe('OperationRoomTypeCreateComponent', () => {
  let component: OperationRoomTypeCreateComponent;
  let fixture: ComponentFixture<OperationRoomTypeCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationRoomTypeCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationRoomTypeCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
