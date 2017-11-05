import { Component } from '@angular/core';
import { AfterViewInit, Directive, ViewChild } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'upload',
    templateUrl: './upload.component.html'
})
export class UploadComponent {
    public _http: Http;

    constructor(http: Http) {
        this._http = http;
    }

    @ViewChild("fileInput") fileInput;
    @ViewChild("fileInputTeacher") fileInputTeacher;

    upload(fileToUpload: any, dataType: string) {
        let input = new FormData();
        input.append("file", fileToUpload);
        input.append("dataType", dataType);

        return this._http
            .post("/api/upload/UploadFileAsync/", input);
    }

    uploadStudentFile(): void {
        let fi = this.fileInput.nativeElement;
        if(fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];
            this.upload(fileToUpload, "Students")
                .subscribe(res => {
                    console.log(res);
                });
        }
    }

    uploadTeacherFile(): void {
        let fi = this.fileInputTeacher.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];
            this.upload(fileToUpload, "Teachers")
                .subscribe(res => {
                    console.log(res);
                });
        }
    }
}