import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'teachers',
    templateUrl: './teachers.component.html'
})
export class TeachersComponent {
    public teachers: Teacher[];

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        http.get(originUrl + '/api/teachers/GetAllTeachersAsync').subscribe(result => {
            this.teachers = result.json() as Teacher[];
        });
    }
}

interface Teacher {
    school: string;
    id: number;
    number: number;
    firstName: string;
}
