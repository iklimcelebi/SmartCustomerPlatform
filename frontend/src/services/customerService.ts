import api from "./api";
import type { Customer } from "../types";

export const getCustomers = async (): Promise<Customer[]> => {
  const response = await api.get("/customers");
  return response.data;
};

export const getCustomerById = async (id: number): Promise<Customer> => {
  const response = await api.get(`/customers/${id}`);
  return response.data;
};

export const searchCustomers = async (
  search: string
): Promise<Customer[]> => {
  const response = await api.get("/customers", {
    params: { search },
  });

  return response.data;
};

export default {
  getCustomers,
  getCustomerById,
  searchCustomers,
};
