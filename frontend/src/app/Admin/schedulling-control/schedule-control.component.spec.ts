import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OperationTypeControlComponent } from '../operation-type-control/admin-operation-type-control.component';


describe('OperationTypeControlComponent', () => {
  let component: OperationTypeControlComponent;
  let fixture: ComponentFixture<OperationTypeControlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperationTypeControlComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationTypeControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
