"use client";

import { useState, useContext } from "react";
import { useRouter } from "next/navigation";
import { loginUser } from "../../utils/requests";
import { UserContext } from "../../context/UserContext";
import styles from "../auth.module.css";
import Link from "next/link";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const { login } = useContext(UserContext);

  async function handleLogin(e) {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const user = await loginUser(username, password);

      if (!user || !user.id) throw new Error("Invalid user data");

      // Save user in context
      login(user);

      // Role-based redirection
      if (user.role?.toLowerCase() === "admin") {
        router.push("/admin");
      } else {
        router.push("/products");
      }

    } catch (error) {
      console.error(error);
      setMessage("Invalid username or password or backend error");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className={styles.authWrapper}>
      <h1 className={styles.title}>Login</h1>
      <form className={styles.authForm} onSubmit={handleLogin}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className={styles.inputField}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className={styles.inputField}
          required
        />
        <button type="submit" className={styles.authButton} disabled={loading}>
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>

      {message && <p className={styles.authMessage}>{message}</p>}

      <p className={styles.authText}>
        Don't have an account?{" "}
        <Link href="/register" className={styles.link}>
          Register
        </Link>
      </p>
    </div>
  );
}
