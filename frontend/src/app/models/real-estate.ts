export interface RealEstate {
    propertyNumber: string;
    state: string;
    city: string;
    neighborhood: string;
    address: string;
    price: string;
    appraisalValue: string;
    discount: string;
    description: string;
    saleMode: string;
    accessLink: string;
}

export type RealEstateList = Array<RealEstate>;