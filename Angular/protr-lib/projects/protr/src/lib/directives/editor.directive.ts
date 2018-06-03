import { Directive, forwardRef, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ProtrConfigurationService } from '../services/configuration.service';
import { Decorator } from '../dtos/decorator';

@Directive({
  selector: '[protrEditor]',
  providers: [
    { provide: ProtrEditorDirective, useClass: forwardRef(() => ProtrEditorDirective) }
  ]
})
export class ProtrEditorDirective implements OnInit {
  @Input() name;

  decorator: Decorator;

  @Output()
  ready: EventEmitter<Decorator> = new EventEmitter();

  constructor(private configurationService: ProtrConfigurationService) {

  }

  ngOnInit(): void {
    this.configurationService
      .getProperties(this.name)
      .then(resp => {
        this.decorator = new Decorator(this.name, resp);
        this.ready.emit(this.decorator);
      });
  }
}
