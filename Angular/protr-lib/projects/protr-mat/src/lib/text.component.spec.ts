import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProtrMatTextComponent } from './text.component';

describe('ProtrMatComponent', () => {
  let component: ProtrMatTextComponent;
  let fixture: ComponentFixture<ProtrMatTextComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProtrMatTextComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProtrMatTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
