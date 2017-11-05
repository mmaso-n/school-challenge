import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public students: Student[];
    public _http: Http;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this._http = http;

        http.get(originUrl + '/api/students/GetAllStudentsAsync').subscribe(result => {
            this.students = result.json() as Student[];
        });
    }

    doDelete(recordToDelete: Student) {
        let input = new FormData();
        input.append("id", recordToDelete.id.toString());
        input.append("number", recordToDelete.number.toString());
        input.append("firstName", recordToDelete.firstName);
        input.append("lastName", recordToDelete.lastName);
        input.append("hasScholarship", String(recordToDelete.hasScholarship));
        input.append("teacherId", recordToDelete.teacherId.toString());

        return this._http
            .post("/api/students/DeleteStudent/", input);        
    }


    deleteStudent(recordToDelete: Student): void {
        if (confirm("Are you sure you want to delete this student?") == true) {
            this.doDelete(recordToDelete)
                .subscribe(res => {
                    console.log(res);
                    location.reload(); // refresh display
                });
        } else {
            // do nothing
        }
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
