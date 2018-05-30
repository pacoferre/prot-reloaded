import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProtrComponent } from './protr.component';

describe('ProtrComponent', () => {
  let component: ProtrComponent;
  let fixture: ComponentFixture<ProtrComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProtrComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProtrComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
