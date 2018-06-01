import { TestBed, inject } from '@angular/core/testing';

import { ProtrMatService } from './protr-mat.service';

describe('ProtrMatService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProtrMatService]
    });
  });

  it('should be created', inject([ProtrMatService], (service: ProtrMatService) => {
    expect(service).toBeTruthy();
  }));
});
