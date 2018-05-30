import { TestBed, inject } from '@angular/core/testing';

import { ProtrService } from './protr.service';

describe('ProtrService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProtrService]
    });
  });

  it('should be created', inject([ProtrService], (service: ProtrService) => {
    expect(service).toBeTruthy();
  }));
});
