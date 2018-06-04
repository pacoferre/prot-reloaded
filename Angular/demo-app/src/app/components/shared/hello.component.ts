import { Component, OnInit, Inject } from '@angular/core';
import { ProtrEditorDirective, IDictionary, IFieldProperty, Decorator } from 'protr';
import { EditorService } from '../../services/editor.service';

@Component({
  selector: 'app-hello',
  templateUrl: './hello.component.html',
})
export class HelloComponent implements OnInit {
  decorator: Decorator;

  constructor(
    private parentComponent: ProtrEditorDirective,
    @Inject('EditorService') private editorService: EditorService
  ) {
  }

  ngOnInit(): void {
    this.editorService
      .currentDecoratorObserver()
      .subscribe(decorator => {
        const that = this;
        this.decorator = decorator;
      });
  }
}
