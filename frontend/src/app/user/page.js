"use client";

import { useContext, useEffect, useState } from "react";
import { UserContext } from "../../context/UserContext";
import { useRouter } from "next/navigation";
import { getUserById, getUserCart } from "../../utils/requests";
import styles from "../page.module.css";

export default function UserPage() {
  const { user, initialized } = useContext(UserContext);
  const router = useRouter();
  const [userData, setUserData] = useState(null);
  const [cartData, setCartData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!initialized) return;

    if (!user || !user.id) {
      router.push("/login");
      return;
    }

    async function fetchData() {
      try {
        const [userRes, cartRes] = await Promise.all([
          getUserById(user.id),
          getUserCart(user.id),
        ]);

        setUserData(userRes);
        setCartData(cartRes);
      } catch (err) {
        console.error(err);
        setError("Failed to load user/cart data");
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [initialized, user, router]);

  if (!initialized || loading)
    return <p style={{ textAlign: "center", marginTop: "2rem" }}>Loading...</p>;

  if (error)
    return (
      <p style={{ textAlign: "center", color: "red", marginTop: "2rem" }}>
        {error}
      </p>
    );

  return (
    <div className={styles.userPage}>
      <h1>Your Profile</h1>

      <div className={styles.profileDetails}>
        <p><strong>Username:</strong> {userData.username}</p>
        <p><strong>Nickname:</strong> {userData.nickname}</p>
        <p><strong>Role:</strong> {userData.role}</p>
        <p><strong>User ID:</strong> {userData.id}</p>
      </div>

      <hr style={{ margin: "1.5rem 0" }} />

      {cartData && cartData.items?.length > 0 ? (
        <div>
          <h2>🛒 Your Cart</h2>
          <ul>
            {cartData.items.map((item, i) => (
              <li key={i}>
                {item.productName} — Qty: {item.quantity}
              </li>
            ))}
          </ul>
        </div>
      ) : (
        <p>You have no items in your cart yet.</p>
      )}
    </div>
  );
}
