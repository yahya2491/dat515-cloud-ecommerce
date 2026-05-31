"use client";

import { useEffect, useState, useContext } from "react";
import { UserContext } from "../../context/UserContext";
import {
  getAllUsers,
  deleteUser,
  updateUser,
  getStats,
  getAllProducts,
  updateProduct,
  deleteProduct,
} from "../../utils/requests";
import { useRouter } from "next/navigation";
import styles from "../page.module.css";

const ADMIN_KEY = "123-admin-key"; // Must match your backend configuration

export default function AdminPage() {
  const { user, initialized } = useContext(UserContext);
  const router = useRouter();
  const [users, setUsers] = useState([]);
  const [products, setProducts] = useState([]);
  const [stats, setStats] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!initialized) return;

    // only admins can access
    if (!user || user.role !== "Admin") {
      router.push("/login");
      return;
    }

    async function fetchData() {
      try {
        const [usersData, statsData, productsData] = await Promise.all([
          getAllUsers(ADMIN_KEY),
          getStats(ADMIN_KEY),
          getAllProducts(),
        ]);

        setUsers(usersData || []);
        setStats(statsData || {});
        setProducts(productsData || []);
      } catch (err) {
        console.error(err);
        setError("Failed to fetch admin data");
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [initialized, user, router]);

  // === USER MANAGEMENT ===
  async function handleDeleteUser(id) {
    if (!confirm("Are you sure you want to delete this user?")) return;
    try {
      await deleteUser(id, ADMIN_KEY);
      setUsers(users.filter((u) => u.id !== id));
    } catch (err) {
      console.error(err);
      alert("Failed to delete user");
    }
  }

async function handleRoleChange(id, newRole) {
  try {
    // Only send the role (and name if required, but keep it the same)
    const userToUpdate = users.find(u => u.id === id);
    if (!userToUpdate) return;

    const payload = {
      name: userToUpdate.nickname, // or userToUpdate.username if 'name' maps to username
      role: newRole
    };

    const updated = await updateUser(id, payload, ADMIN_KEY);

    // Update state
    setUsers(users.map(u => (u.id === id ? { ...u, role: newRole } : u)));
  } catch (err) {
    console.error(err);
    alert("Failed to update user role");
  }
}
  // === PRODUCT MANAGEMENT ===
  async function handleDeleteProduct(id) {
    if (!confirm("Are you sure you want to delete this product?")) return;
    try {
      await deleteProduct(id, ADMIN_KEY);
      setProducts(products.filter((p) => p.id !== id));
    } catch (err) {
      console.error(err);
      alert("Failed to delete product");
    }
  }

  async function handleUpdateProduct(product) {
    try {
      const updated = await updateProduct(product.id, product, ADMIN_KEY);
      setProducts(products.map((p) => (p.id === product.id ? updated : p)));
      alert("Product updated successfully!");
    } catch (err) {
      console.error(err);
      alert("Failed to update product");
    }
  }

  if (loading) return <p>Loading admin data...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div className={styles.pageWrapper}>
      <h1>Admin Dashboard</h1>

      {/* === Stats === */}
      <section>
        <h2>Stats</h2>
        <pre>{JSON.stringify(stats, null, 2)}</pre>
      </section>

      {/* === Users Table === */}
      <section>
        <h2>Users</h2>
        <table border="1" cellPadding="8">
          <thead>
            <tr>
              <th>Username</th>
              <th>Nickname</th>
              <th>Role</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.username}</td>
                <td>{u.nickname}</td>
                <td>
                  <select
                    value={u.role}
                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                  >
                    <option value="User">User</option>
                    <option value="Admin">Admin</option>
                  </select>
                </td>
                <td>
                  <button onClick={() => handleDeleteUser(u.id)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      {/* === Products Table === */}
      <section style={{ marginTop: "2rem" }}>
        <h2>Products</h2>
        <table border="1" cellPadding="8">
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
               <th>Category</th>
              <th>Price</th>
              <th>Stock</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {products.map((p) => (
              <ProductRow
                key={p.id}
                product={p}
                onDelete={handleDeleteProduct}
                onSave={handleUpdateProduct}
              />
            ))}
          </tbody>
        </table>
      </section>
    </div>
  );
}

// Reusable product row component
function ProductRow({ product, onDelete, onSave }) {
  const [edit, setEdit] = useState(false);
  const [form, setForm] = useState({ ...product });

  function handleChange(e) {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  }

  return (
    <tr>
      <td>
        {edit ? (
          <input name="name" value={form.name} onChange={handleChange} />
        ) : (
          product.name
        )}
      </td>
      <td>
        {edit ? (
          <input name="description" value={form.description} onChange={handleChange} />
        ) : (
          product.description
        )}
      </td>
      <td>
  {edit ? (
    <input name="category" value={form.category} onChange={handleChange} />
  ) : (
    product.category
  )}
</td>

      <td>
        {edit ? (
          <input
            name="price"
            type="number"
            value={form.price}
            onChange={handleChange}
          />
        ) : (
          `$${product.price.toFixed(2)}`
        )}
      </td>
      <td>
        {edit ? (
          <input
            name="stock"
            type="number"
            value={form.stock || 0}
            onChange={handleChange}
          />
        ) : (
          product.stock || 0
        )}
      </td>
      <td>
        {edit ? (
          <>
            <button onClick={() => onSave(form)}>Save</button>
            <button onClick={() => setEdit(false)}>Cancel</button>
          </>
        ) : (
          <>
            <button onClick={() => setEdit(true)}>Edit</button>
            <button onClick={() => onDelete(product.id)}>Delete</button>
          </>
        )}
      </td>
    </tr>
  );
}
