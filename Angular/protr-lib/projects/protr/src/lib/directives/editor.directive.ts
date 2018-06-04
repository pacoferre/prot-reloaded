import { Directive, forwardRef, Input, OnInit, Inject } from '@angular/core';
import { ProtrEditorService } from '../services/editor.service';

@Directive({
  selector: '[protrEditor]',
  providers: [
    { provide: ProtrEditorDirective, useClass: forwardRef(() => ProtrEditorDirective) }
  ]
})
export class ProtrEditorDirective implements OnInit {
  @Input() name: string;

  constructor(@Inject('EditorService') private protrEditorService: ProtrEditorService) {
  }

  ngOnInit(): void {
    this.protrEditorService
      .init(this.name);
  }
}
