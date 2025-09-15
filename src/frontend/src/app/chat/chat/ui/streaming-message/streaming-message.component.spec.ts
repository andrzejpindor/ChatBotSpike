import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StreamingMessageComponent } from './streaming-message.component';

describe('StreamingMessageComponent', () => {
  let component: StreamingMessageComponent;
  let fixture: ComponentFixture<StreamingMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreamingMessageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StreamingMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
