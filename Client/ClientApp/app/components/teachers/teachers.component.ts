import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'teachers',
    templateUrl: './teachers.component.html'
})
export class TeachersComponent {
    public teachers: Teacher[];
    public _http: Http;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this._http = http;

        http.get(originUrl + '/api/teachers/GetAllTeachersAsync').subscribe(result => {
            this.teachers = result.json() as Teacher[];
        });
    }

    doDelete(recordToDelete: Teacher) {
        let input = new FormData();
        input.append("id", recordToDelete.id.toString());
        input.append("firstName", recordToDelete.firstName);
        input.append("lastName", recordToDelete.lastName);

        return this._http
            .post("/api/teachers/DeleteTeacher/", input);
    }

    deleteTeacher(recordToDelete: Teacher): void {
        if (confirm("Are you sure you want to delete this teacher?") == true) {
            this.doDelete(recordToDelete)
                .subscribe(res => {
                    console.log(res);
                    location.reload(); // refresh display
                });
        }
        else {
            // do nothing
        }
    }
}

interface Teacher {
    school: string;
    id: number;
    firstName: string;
    lastName: string
}
