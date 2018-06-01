import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProtrMatComponent } from './protr-mat.component';

describe('ProtrMatComponent', () => {
  let component: ProtrMatComponent;
  let fixture: ComponentFixture<ProtrMatComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProtrMatComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProtrMatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
