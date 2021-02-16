import { TestBed } from '@angular/core/testing';

import { MemberDetailsResolver } from './member-details.resolver';

describe('MemberDetailsResolverResolver', () => {
  let resolver: MemberDetailsResolver;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    resolver = TestBed.inject(MemberDetailsResolver);
  });

  it('should be created', () => {
    expect(resolver).toBeTruthy();
  });
});
