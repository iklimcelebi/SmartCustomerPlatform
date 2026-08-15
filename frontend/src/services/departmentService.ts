import api from "./api";
import type {
  Department,
  DepartmentCreateRequest,
  DepartmentUpdateRequest,
} from "../types";

export const getDepartments = async (): Promise<Department[]> => {
  const response = await api.get("/departments");
  return response.data;
};

export const getDepartmentById = async (
  id: string
): Promise<Department> => {
  const response = await api.get(`/departments/${id}`);
  return response.data;
};

export const createDepartment = async (
  data: DepartmentCreateRequest
): Promise<Department> => {
  const response = await api.post("/departments", data);
  return response.data;
};

export const updateDepartment = async (
  id: string,
  data: DepartmentUpdateRequest
): Promise<Department> => {
  const response = await api.put(`/departments/${id}`, data);
  return response.data;
};

export const deleteDepartment = async (
  id: string
): Promise<void> => {
  await api.delete(`/departments/${id}`);
};

export const transferTicket = async (
  ticketId: string | number,
  departmentId: string
) => {
  const response = await api.put(
    `/tickets/${ticketId}/transfer`,
    { departmentId }
  );

  return response.data;
};

export default {
  getDepartments,
  getDepartmentById,
  createDepartment,
  updateDepartment,
  deleteDepartment,
  transferTicket,
};
