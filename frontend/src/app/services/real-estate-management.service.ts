import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../src/environments/environment'
import { Login } from '../models/login';
import { FormGroup } from '@angular/forms';
import { RealEstate, RealEstateList } from '../models/real-estate';
import { Observable } from 'rxjs';
import { BaseResponse } from '../models/base-response';
import { SessionStorageService } from './session-storage.service';

const BASE_URL = environment.base_url;

@Injectable({
    providedIn: 'root'
})
export class RealEstateService {

    constructor(private httpClient: HttpClient, private sessionStorageService: SessionStorageService) { }

    public login(formGroup: FormGroup): Observable<Login> {
        return this.httpClient.post<Login>(`${BASE_URL}/login`, formGroup.value);
    }

    public bulkAdd(formData: FormData): Observable<BaseResponse<string>> {
        const headers = this.getHeaders();
        return this.httpClient.post<BaseResponse<string>>(`${BASE_URL}/add-from-csvfile`, formData, { headers: headers });
    }

    public getById(id: string): Observable<BaseResponse<RealEstate>> {
        const headers = this.getHeaders();
        const params = new HttpParams().set('id', id);
        return this.httpClient.get<BaseResponse<RealEstate>>(`${BASE_URL}/getById`, { params: params, headers: headers });
    }

    public get(
        page: number,
        pageSize: number,
        state: string | undefined = undefined,
        city: string | undefined = undefined,
        saleMode: string | undefined = undefined
    ): Observable<BaseResponse<RealEstateList>> {
        const headers = this.getHeaders();
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (state)
            params = params.set('state', state);

        if (city)
            params = params.set('city', city);

        if (saleMode)
            params = params.set('saleMode', saleMode);

        return this.httpClient.get<BaseResponse<RealEstateList>>(`${BASE_URL}/get`, { params: params, headers: headers });
    }

    private getHeaders() {
        const token = this.sessionStorageService.getTokenFromSessionStorage();
        return new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });
    }
}
