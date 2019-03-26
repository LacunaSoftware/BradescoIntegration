import { BradescoBuyerInfo } from "./bradesco-buyer-info";

export class PurchaseRequest {
  buyerInfo: BradescoBuyerInfo;
  value: number;
  bankBilletExpiration: Date;

}
