import api from "./api";
import type { Category, Subcategory } from "../types";

export interface CreateCategoryRequest {
  code: string;
  name: string;
  description: string;
  isActive: boolean;
}

export const getCategories = async (): Promise<Category[]> => {
  const response = await api.get("/TicketCategories");
  return response.data;
};

export const getCategoryById = async (
  id: string
): Promise<Category> => {
  const response = await api.get(
    `/TicketCategories/${id}`
  );

  return response.data;
};

export const createCategory = async (
  data: CreateCategoryRequest
): Promise<string> => {
  const response = await api.post(
    "/TicketCategories",
    data
  );

  return response.data;
};

export const getSubcategories = async (
  categoryId: string
): Promise<Subcategory[]> => {
  const response = await api.get(
    `/TicketSubCategories/category/${categoryId}`
  );

  return response.data;
};

export const createSubcategory = async (data: {
  name: string;
  description: string;
  categoryId: string;
}): Promise<Subcategory> => {
  const response = await api.post(
    "/TicketSubCategories",
    data
  );

  return response.data;
};

export default {
  getCategories,
  getCategoryById,
  createCategory,
  getSubcategories,
  createSubcategory,
};