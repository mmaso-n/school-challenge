import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public students: Student[];

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        http.get(originUrl + '/api/students/GetAllStudents').subscribe(result => {
            this.students = result.json() as Student[];
        });
    }
}

interface Student {
    school: string;
    id: number;
    number: number;
    firstName: string;
    lastName: string;
    hasScholarship: boolean;
    teacherId: number
}
