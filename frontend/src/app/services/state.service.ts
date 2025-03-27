import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { States } from '../models/state';
import { firstValueFrom } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StateService {
    constructor(private httpClient: HttpClient) { }

    private async getStatesBase(): Promise<States> {
        try {
            return await firstValueFrom(this.httpClient.get<States>('./assets/states-cities.json'));
        } catch (error) {
            console.error('Error fetching states:', error);
            return [];
        }
    }

    async getStates(): Promise<States> {
        return await this.getStatesBase();
    }

    async getCitiesByUf(uf: string): Promise<string[]> {
        const states = await this.getStatesBase();
        const state = states.find(x => x.uf === uf);
        return state?.cities || [];
    }
}